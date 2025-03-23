using Necli.Entities; // Importa las entidades del proyecto.
using Necli.Persistencia; // Importa la capa de persistencia, donde se gestionan las transacciones en la base de datos.

namespace Necli.WebApi.Services
{
    // Servicio encargado de gestionar las transacciones dentro de la aplicación.
    public class GestionarTransacciones
    {
        private readonly TransaccionesRepository _transaccionesRepository; // Repositorio para interactuar con la base de datos.

        // Constructor que recibe el repositorio mediante inyección de dependencias.
        public GestionarTransacciones(TransaccionesRepository transaccionesRepository)
        {
            _transaccionesRepository = transaccionesRepository;
        }

        // Método para registrar una transacción en la base de datos.
        public bool RegistrarTransaccion(Transacciones transaccion)
        {
            return _transaccionesRepository.RegistrarTransaccion(transaccion);
        }

        // Método para consultar una transacción específica en la base de datos.
        public Transacciones? ConsultarTransaccion(int numeroTransaccion)
        {
            return _transaccionesRepository.ConsultarTransaccion(numeroTransaccion);
        }
    }
}
