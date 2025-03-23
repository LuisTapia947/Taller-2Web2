using Microsoft.Data.SqlClient; // Importa la librería para manejar conexiones a SQL Server.
using Necli.Entities; // Importa las entidades del proyecto.
using System.Data; // Importa funcionalidades para manejar datos.

namespace Necli.Persistencia
{
    // Repositorio encargado de gestionar las operaciones en la base de datos relacionadas con las transacciones.
    public class TransaccionesRepository
    {
        // Cadena de conexión a la base de datos (debe almacenarse de forma segura en entornos productivos).
        private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli; User ID=sa; Password=12345; TrustServerCertificate=True;";

        // Método para registrar una transacción en la base de datos.
        public bool RegistrarTransaccion(Transacciones transaccion)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaConexion)) // Establece la conexión con la base de datos.
                {
                    conexion.Open(); // Abre la conexión a la base de datos.
                    using (var transaccionSQL = conexion.BeginTransaction()) // Inicia una transacción SQL para asegurar atomicidad.
                    {
                        try
                        {
                            // Verificar si las cuentas de origen y destino existen antes de procesar la transacción.
                            string verificarCuentas = @"
                                SELECT COUNT(*) FROM Cuentas WHERE Numero = @NumeroCuentaOrigen;
                                SELECT COUNT(*) FROM Cuentas WHERE Numero = @NumeroCuentaDestino;
                            ";

                            using (var comando = new SqlCommand(verificarCuentas, conexion, transaccionSQL))
                            {
                                comando.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                                comando.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);

                                using (var reader = comando.ExecuteReader()) // Ejecuta la consulta.
                                {
                                    reader.Read(); // Lee el primer resultado (cuenta de origen).
                                    int cuentaOrigenExiste = reader.GetInt32(0);
                                    reader.NextResult(); // Pasa al siguiente resultado (cuenta de destino).
                                    reader.Read();
                                    int cuentaDestinoExiste = reader.GetInt32(0);

                                    if (cuentaOrigenExiste == 0 || cuentaDestinoExiste == 0) // Si alguna cuenta no existe, cancela la transacción.
                                    {
                                        transaccionSQL.Rollback();
                                        return false;
                                    }
                                }
                            }

                            // Verificar que la cuenta de origen tenga saldo suficiente.
                            string consultaSaldo = "SELECT Saldo FROM Cuentas WHERE Numero = @NumeroCuentaOrigen";
                            decimal saldoOrigen;

                            using (var comandoSaldo = new SqlCommand(consultaSaldo, conexion, transaccionSQL))
                            {
                                comandoSaldo.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                                saldoOrigen = (decimal)comandoSaldo.ExecuteScalar(); // Obtiene el saldo actual de la cuenta.
                            }

                            if (saldoOrigen < transaccion.Monto) // Si el saldo es insuficiente, cancela la transacción.
                            {
                                transaccionSQL.Rollback();
                                return false;
                            }

                            // Descontar saldo de la cuenta de origen.
                            string actualizarSaldoOrigen = "UPDATE Cuentas SET Saldo = Saldo - @Monto WHERE Numero = @NumeroCuentaOrigen";
                            using (var comandoDescontar = new SqlCommand(actualizarSaldoOrigen, conexion, transaccionSQL))
                            {
                                comandoDescontar.Parameters.AddWithValue("@Monto", transaccion.Monto);
                                comandoDescontar.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                                comandoDescontar.ExecuteNonQuery(); // Ejecuta la actualización.
                            }

                            // Aumentar saldo en la cuenta de destino.
                            string actualizarSaldoDestino = "UPDATE Cuentas SET Saldo = Saldo + @Monto WHERE Numero = @NumeroCuentaDestino";
                            using (var comandoAumentar = new SqlCommand(actualizarSaldoDestino, conexion, transaccionSQL))
                            {
                                comandoAumentar.Parameters.AddWithValue("@Monto", transaccion.Monto);
                                comandoAumentar.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);
                                comandoAumentar.ExecuteNonQuery(); // Ejecuta la actualización.
                            }

                            // Registrar la transacción en la tabla Transacciones.
                            string insertarTransaccion = @"
                                INSERT INTO Transacciones (NumeroCuentaOrigen, NumeroCuentaDestino, Monto, Tipo)
                                OUTPUT INSERTED.NumeroTransaccion, INSERTED.FechaTransaccion
                                VALUES (@NumeroCuentaOrigen, @NumeroCuentaDestino, @Monto, @Tipo)";

                            using (var comandoInsertar = new SqlCommand(insertarTransaccion, conexion, transaccionSQL))
                            {
                                comandoInsertar.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                                comandoInsertar.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);
                                comandoInsertar.Parameters.AddWithValue("@Monto", transaccion.Monto);
                                comandoInsertar.Parameters.AddWithValue("@Tipo", transaccion.Tipo);

                                using (var reader = comandoInsertar.ExecuteReader()) // Ejecuta la inserción y obtiene los valores generados automáticamente.
                                {
                                    if (reader.Read())
                                    {
                                        transaccion.NumeroTransaccion = reader.GetInt32(0); // Asigna el Número de Transacción.
                                        transaccion.FechaTransaccion = reader.GetDateTime(1); // Asigna la Fecha de Transacción.
                                    }
                                }
                            }

                            transaccionSQL.Commit(); // Confirma la transacción si todo fue exitoso.
                            return true;
                        }
                        catch
                        {
                            transaccionSQL.Rollback(); // Revierte la transacción en caso de error.
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false; // Retorna false si hay un error en la conexión o en la transacción.
            }
        }

        // Método para consultar una transacción por su número.
        public Transacciones? ConsultarTransaccion(int numeroTransaccion)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaConexion)) // Establece la conexión con la base de datos.
                {
                    string sql = "SELECT * FROM Transacciones WHERE NumeroTransaccion = @NumeroTransaccion";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@NumeroTransaccion", numeroTransaccion);
                        conexion.Open();

                        using (var lector = comando.ExecuteReader()) // Ejecuta la consulta.
                        {
                            if (lector.Read()) // Si encuentra la transacción, la convierte en un objeto.
                            {
                                return new Transacciones
                                {
                                    NumeroTransaccion = Convert.ToInt32(lector["NumeroTransaccion"]),
                                    FechaTransaccion = Convert.ToDateTime(lector["FechaTransaccion"]),
                                    NumeroCuentaOrigen = Convert.ToInt32(lector["NumeroCuentaOrigen"]),
                                    NumeroCuentaDestino = Convert.ToInt32(lector["NumeroCuentaDestino"]),
                                    Monto = Convert.ToDecimal(lector["Monto"]),
                                    Tipo = (Tipo)Convert.ToInt32(lector["Tipo"]) // Convierte el campo Tipo al enum correspondiente.
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar transacción: {ex.Message}"); // Captura y muestra el error en consola.
                return null;
            }

            return null; // Retorna null si no encuentra la transacción.
        }
    }
}
