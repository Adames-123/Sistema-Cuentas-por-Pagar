using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Documentos_por_Pagar.Models
{
    public class DocumentoPorPagar

    {
        [Key]
        public int IdDocumento { get; set; }
        public int IdProveedor { get; set; }

        [ForeignKey("IdProveedor")]
        public Proveedor ?Proveedor { get; set; }
        public int IdConcepto { get; set; }

        [ForeignKey("IdConcepto")]
        public Concepto? Concepto { get; set; }
        public decimal Monto { get; set; }
        public string ?NumeroDocumento { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "Factura inválida")]

        public string? NumeroFactura { get; set; }

        public DateTime ?FechaDocumento { get; set; }
        public DateTime ?FechaRegistro { get; set; }

        public Boolean Estado { get; set; } = true;

        /// <summary>
        /// ID del asiento contable generado en Contabilidad.
        /// null = pendiente de contabilizar, valor = ya contabilizado.
        /// </summary>
        public int? IdAsiento { get; set; }
    }
}
