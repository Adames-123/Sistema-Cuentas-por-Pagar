namespace Sistema_Documentos_por_Pagar.Models
{
    public class AsientoContable
    {
        public string ?Descripcion { get; set; }
        public int IdAuxiliar { get; set; }
        public DateTime Fecha { get; set; }
        public string ?Origen { get; set; }
        public List<DetalleAsiento> ?Detalles { get; set; }
    }
}
