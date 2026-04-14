namespace Sistema_Documentos_por_Pagar.Models
{
    public class RespuestaAsiento
    {
        public bool Exitoso { get; set; }
        public int? IdAsiento { get; set; }
        public string? Mensaje { get; set; }
    }
}
