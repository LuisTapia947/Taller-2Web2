// Se importan los espacios de nombres necesarios
using Necli.Entities; // Contiene las entidades del sistema, como Cuenta
using Necli.Persistencia; // Contiene el repositorio que interactúa con la base de datos

namespace Necli.WebApi.Services; // Define el namespace del servicio

// Clase que maneja la lógica de negocio de las cuentas
public class GestionarCuentas
{
    // Instancia del repositorio de cuentas para interactuar con la base de datos
    private readonly CuentaRepository _cuentaRepository;

    // Constructor para inyectar el repositorio de cuentas
    public GestionarCuentas(CuentaRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    // Método para crear una cuenta validando su saldo antes de insertarla
    public bool CrearCuenta(Cuenta cuenta)
    {
        // Se valida que el saldo inicial esté dentro del rango permitido (entre 1,000 y 5,000,000 COP)
        if (cuenta.Saldo < 1000 || cuenta.Saldo > 5000000)
        {
            return false; // Retorna false si el saldo es inválido
        }

        // Llama al repositorio para insertar la cuenta en la base de datos
        return _cuentaRepository.CrearCuenta(cuenta);
    }

    // Método para consultar una cuenta por su número
    public CuentaDTO? ConsultarCuenta(string numero)
    {
        // Llama al repositorio para obtener los datos de la cuenta
        return _cuentaRepository.ConsultarCuenta(numero);
    }

    // Método para eliminar una cuenta con validaciones
    public bool EliminarCuenta(string numero)
    {
        // Se obtiene la cuenta desde la base de datos
        var cuenta = _cuentaRepository.ConsultarCuenta(numero);

        // Si la cuenta no existe, se retorna false
        if (cuenta == null)
        {
            return false;
        }

        // Verifica que el saldo de la cuenta no sea mayor a 50,000 COP antes de eliminarla
        if (cuenta.Saldo > 50000)
        {
            return false; // No se puede eliminar una cuenta con saldo mayor a 50,000 COP
        }

        // Si pasa las validaciones, se elimina la cuenta y retorna true si tuvo éxito
        return _cuentaRepository.EliminarCuenta(numero);
    }
}
