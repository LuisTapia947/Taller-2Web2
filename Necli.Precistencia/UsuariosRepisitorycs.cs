using Microsoft.Data.SqlClient; 
using Necli.Entities;

namespace Necli.Persistencia; 

public class UsuarioRepository
{
    private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli;User ID=sa;Password=12345; TrustServerCertificate=True;";
    public bool RegistrarUsuario(Usuario usuario)
    {
        try
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = "INSERT INTO Usuarios (Id, NombreUsuario, ApellidoUsuario, Contraseña, Correo) " +
                             "VALUES (@Id, @Nombre, @Apellido, @Contraseña, @Correo)";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", usuario.Id);  // Asegúrate de tener un Id único
                    comando.Parameters.AddWithValue("@Nombre", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@Apellido", usuario.ApellidoUsuario);
                    comando.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);  // Contraseña como string
                    comando.Parameters.AddWithValue("@Correo", usuario.Correo);

                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;  // Si se ejecuta correctamente, retorna true
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar usuario: {ex.Message}");
            return false;  // Retorna false si ocurre un error
        }
    }



    public UsuariosDTO? ConsultarUsuario(string id)
    {
        try
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = "SELECT Id, NombreUsuario, ApellidoUsuario, Correo FROM Usuarios WHERE Id = @Id";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new UsuariosDTO
                            {
                                Id = lector["Id"].ToString(),
                                NombreUsuario = lector["NombreUsuario"].ToString(),
                                ApellidoUsuario = lector["ApellidoUsuario"].ToString(),
                                Correo = lector["Correo"].ToString()
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al consultar usuario: {ex.Message}");
        }
        return null;
    }

    public bool EliminarUsuario(string id)
    {
        try
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = "DELETE FROM Usuarios WHERE Id = @Id";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", id);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            return false;
        }
    }
    public bool ActualizarUsuario(string id, Usuario usuario)
    {
        try
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = @"UPDATE Usuarios 
                           SET NombreUsuario = @Nombre, 
                               ApellidoUsuario = @Apellido, 
                               Contraseña = @Contraseña 
                           WHERE Id = @Id";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@Apellido", usuario.ApellidoUsuario);
                    comando.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                    comando.Parameters.AddWithValue("@Id", id);

                    conexion.Open();

                    // Verifica cuántas filas fueron afectadas
                    int filasAfectadas = comando.ExecuteNonQuery();
                    Console.WriteLine($"Filas afectadas: {filasAfectadas}");

                    return filasAfectadas > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
            return false;
        }
    }

}
