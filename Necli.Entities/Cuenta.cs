namespace Necli.Entities
{
    public class Cuenta
    {
        public long Numero { get; set; } 
        public string UsuarioId { get; set; } 
        public decimal Saldo { get; set; } 
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow; 
    }
}
