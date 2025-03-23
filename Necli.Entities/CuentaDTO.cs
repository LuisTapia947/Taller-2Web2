using System.ComponentModel.DataAnnotations;

namespace Necli.Entities;

public class CuentaDTO
{
    // Número de cuenta, obligatorio
    [Required]
    public string Numero { get; set; }

    // Saldo de la cuenta con restricciones de mínimo y máximo
    [Range(1000, 5000000, ErrorMessage = "El saldo debe estar entre 1,000 y 5,000,000 COP.")]
    public decimal Saldo { get; set; }
}
