namespace Sistema_Documentos_por_Pagar.Models.Contabilidad
{
    // ─── Objetos anidados requeridos por la API de Contabilidad ───────────────

    public class ContabilidadAuxiliar
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }

    public class ContabilidadTipoCuenta
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Origen { get; set; }   // "DEBITO" | "CREDITO"
        public bool Estado { get; set; } = true;
    }

    public class ContabilidadCuenta
    {
        public int? Id { get; set; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool PermiteMovimiento { get; set; } = true;
        public ContabilidadTipoCuenta? Tipo { get; set; }
        public int Nivel { get; set; } = 1;
        public decimal? Balance { get; set; }
        public string? CuentaMayor { get; set; }
        public bool Estado { get; set; } = true;
    }

    public class ContabilidadMoneda
    {
        public int? Id { get; set; }
        public string CodigoIso { get; set; } = "DOP";
        public string Nombre { get; set; } = "Peso Dominicano";
        public string Simbolo { get; set; } = "RD$";
        public string Descripcion { get; set; } = "Peso Dominicano";
        public decimal TasaCambio { get; set; } = 1;
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }

    public class ContabilidadDetalle
    {
        public int? Id { get; set; }
        public ContabilidadCuenta? Cuenta { get; set; }
        public string? TipoMovimiento { get; set; }  // "Debito" | "Credito"
        public decimal Monto { get; set; }
    }

    // ─── Request completo POST /api/asientos ─────────────────────────────────

    public class AsientoContableRequest
    {
        public string? Descripcion { get; set; }
        public ContabilidadAuxiliar? Auxiliar { get; set; }
        public string? FechaAsiento { get; set; }   // "yyyy-MM-dd"
        public decimal MontoTotal { get; set; }
        public List<ContabilidadDetalle>? Detalles { get; set; }
        public bool Estado { get; set; } = true;
        public ContabilidadMoneda? Moneda { get; set; }
        public decimal TasaCambio { get; set; } = 1;
        public decimal MontoTotalDop { get; set; }
    }

    // ─── Respuesta exitosa POST /api/asientos ────────────────────────────────
    // Campos confirmados desde el Swagger del grupo de Contabilidad

    public class AsientoContableResponse
    {
        public int Id { get; set; }                          // ← el que guardamos en IdAsiento
        public string? Descripcion { get; set; }
        public ContabilidadAuxiliar? Auxiliar { get; set; }
        public string? FechaAsiento { get; set; }
        public decimal MontoTotal { get; set; }
        public List<ContabilidadDetalle>? Detalles { get; set; }
        public bool Estado { get; set; }
        public ContabilidadMoneda? Moneda { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal MontoTotalDop { get; set; }
    }
}
