using System.ComponentModel.DataAnnotations;

namespace Sistema_Documentos_por_Pagar.Models
{
    public class DetalleAsiento
    {
        public int IdCuenta { get; set; }
        public string? Descripcion { get; set; }
        [Required]
        public string? TipoMovimiento { get; set; } 
        public decimal Monto { get; set; }
    }
}
