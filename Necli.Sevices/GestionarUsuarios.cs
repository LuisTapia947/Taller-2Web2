using Necli.Entities; 
using Necli.Persistencia; // Espacio de nombres donde está definido el repositorio de acceso a datos

namespace Necli.WebApi.Services; // Define el namespace del servicio


public class GestionarUsuarios
{
    
    private readonly UsuarioRepository _usuarioRepository = new();


    public bool RegistrarUsuario(Usuario usuario)
    {
        // Validación de datos obligatorios para evitar registros con información incompleta
        if (string.IsNullOrEmpty(usuario.NombreUsuario) || // Verifica que el nombre no sea nulo o vacío
            string.IsNullOrEmpty(usuario.ApellidoUsuario) || // Verifica que el apellido no sea nulo o vacío
            string.IsNullOrEmpty(usuario.Contraseña) || // Verifica que la contraseña no sea nula o vacía
            string.IsNullOrEmpty(usuario.Correo)) // Verifica que el correo no sea nulo o vacío
        {
            return false; // Retorna false si algún campo está vacío
        }

        // Llama al repositorio para registrar el usuario en la base de datos
        return _usuarioRepository.RegistrarUsuario(usuario);
    }

    // Método para consultar un usuario por su ID
    public UsuariosDTO? ConsultarUsuario(string id)
    {
        // Llama al repositorio para obtener los datos del usuario
        return _usuarioRepository.ConsultarUsuario(id);
    }

    // Método para eliminar un usuario por su ID
    public bool EliminarUsuario(string id)
    {
        // Llama al repositorio para eliminar el usuario de la base de datos
        return _usuarioRepository.EliminarUsuario(id);
    }

    // Método para actualizar los datos de un usuario
    public bool ActualizarUsuario(string id, Usuario usuario)
    {
        // Llama al repositorio para actualizar los datos del usuario en la base de datos
        return _usuarioRepository.ActualizarUsuario(id, usuario);
    }
}
