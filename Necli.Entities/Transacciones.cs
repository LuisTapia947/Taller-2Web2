
namespace Necli.Entities
{
    public class Transacciones
    {
        public int NumeroTransaccion { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public int NumeroCuentaOrigen { get; set; }
        public int NumeroCuentaDestino { get; set; }
        public decimal Monto { get; set; }
        public Tipo Tipo { get; set; } 
    }
}
