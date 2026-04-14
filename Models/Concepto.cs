using System.ComponentModel.DataAnnotations;




namespace Sistema_Documentos_por_Pagar.Models
{
    public class Concepto
    {
        [Key]
        public int IdConcepto { get; set; }

        public string? Descripcion { get; set; }
        public Boolean Estado { get; set; } = true;
    }
}
