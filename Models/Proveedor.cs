using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sistema_Documentos_por_Pagar.Models
{
    public class Proveedor
    {
        [Key]
        public  int IdProveedor { get; set; }
        public string ?Nombre { get; set; }
        public string ?TipoPersona { get; set; }

        [RegularExpression(@"^\d{3}-?\d{7}-?\d{1}$", ErrorMessage = "Cédula inválida")]
        public string ?CedulaRNC { get; set; }
        public decimal ?Balance { get; set; }
        public bool Estado { get; set; } = true;


    }
}
