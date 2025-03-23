// Importa las librerías necesarias
using Microsoft.Data.SqlClient; // Permite la conexión y manipulación de datos en SQL Server
using Necli.Entities; // Contiene las entidades del sistema, como Cuenta y CuentaDTO

namespace Necli.Persistencia; // Define el namespace para la capa de persistencia

// Clase que maneja la persistencia de datos de las cuentas
public class CuentaRepository
{
    // Cadena de conexión a la base de datos
    private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli;User ID=sa;Password=12345; TrustServerCertificate=True;";
   
    public bool CrearCuenta(Cuenta cuenta)
    {
        try
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                // Obtener el número de cuenta secuencial
                string sqlSecuencial = "SELECT ISNULL(MAX(Numero), 0) + 1 FROM Cuentas"; // Obtiene el número máximo de cuenta y le suma 1

                long numeroCuenta = 0;
                using (var comandoSecuencial = new SqlCommand(sqlSecuencial, conexion))
                {
                    conexion.Open();
                    numeroCuenta = Convert.ToInt64(comandoSecuencial.ExecuteScalar()); // Obtiene el siguiente número disponible
                }

                // Ahora se inserta la cuenta con el número generado
                string sqlInsert = "INSERT INTO Cuentas (Numero, UsuarioId, Saldo, FechaCreacion) VALUES (@Numero, @UsuarioId, @Saldo, @FechaCreacion)";

                using (var comandoInsert = new SqlCommand(sqlInsert, conexion))
                {
                    comandoInsert.Parameters.AddWithValue("@Numero", numeroCuenta); // Número de cuenta generado
                    comandoInsert.Parameters.AddWithValue("@UsuarioId", cuenta.UsuarioId); // El Id del usuario
                    comandoInsert.Parameters.AddWithValue("@Saldo", cuenta.Saldo); // Saldo de la cuenta
                    comandoInsert.Parameters.AddWithValue("@FechaCreacion", cuenta.FechaCreacion); // Fecha de creación

                    return comandoInsert.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear la cuenta: {ex.Message}");
            return false;
        }
    }



    // Método para consultar una cuenta por su número
    public CuentaDTO? ConsultarCuenta(string numero)
    {
        try
        {
            // Se establece la conexión con la base de datos
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                // Query SQL para obtener los datos de una cuenta específica
                string sql = "SELECT Numero, Saldo FROM Cuentas WHERE Numero = @Numero";

                // Se prepara el comando SQL
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Numero", numero); // Parámetro número de cuenta

                    // Se abre la conexión
                    conexion.Open();

                    // Ejecuta la consulta y obtiene los resultados
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read()) // Si hay resultados
                        {
                            return new CuentaDTO
                            {
                                Numero = lector["Numero"]?.ToString() ?? string.Empty, // Obtiene el número de cuenta
                                Saldo = lector["Saldo"] != DBNull.Value ? Convert.ToDecimal(lector["Saldo"]) : 0 // Convierte el saldo
                            };
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return null; // Si ocurre un error, retorna null
        }
        return null; // Retorna null si no se encuentra la cuenta
    }

    // Método para eliminar una cuenta por su número
    public bool EliminarCuenta(string numero)
    {
        try
        {
            // Se establece la conexión con la base de datos
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                // Query SQL para eliminar una cuenta específica
                string sql = "DELETE FROM Cuentas WHERE Numero = @Numero";

                // Se prepara el comando SQL
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Numero", numero); // Parámetro número de cuenta

                    // Se abre la conexión
                    conexion.Open();

                    // Ejecuta la consulta y devuelve true si al menos una fila fue afectada
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception)
        {
            return false; // Si ocurre un error, retorna false
        }
    }
}
