using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sistema_Documentos_por_Pagar.Models;
using Sistema_Documentos_por_Pagar.Models.Contabilidad;

namespace Sistema_Documentos_por_Pagar.Services
{
    /// <summary>
    /// Convierte DateTime al formato "yyyy-MM-ddTHH:mm:ss" que espera
    /// Java LocalDateTime — sin offset ni decimales extras.
    /// </summary>
    public class JavaLocalDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
    }

    public class ContabilidadService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContabilidadService> _logger;

        // Auxiliar fijo del grupo CxP (ID 6 segun catalogo de Contabilidad)
        private const int ID_AUXILIAR_CXP = 6;
        private const string NOMBRE_AUXILIAR_CXP = "Cuentas por Pagar";

        // Cuentas confirmadas desde GET /api/cuentas-contables
        // ID 1 = "2101-01" Cuentas por Pagar Proveedores -> Debito
        // ID 2 = "1102-01" Banco Cuenta Cheques          -> Credito
        private static readonly ContabilidadCuenta CUENTA_CXP_PROVEEDOR = new()
        {
            Id = 1,
            Codigo = "2101-01",
            Nombre = "Cuentas por Pagar Proveedores",
            Descripcion = "Obligaciones con proveedores",
            PermiteMovimiento = true,
            Nivel = 1,
            Estado = true,
            Tipo = new ContabilidadTipoCuenta
            {
                Id = 1,
                Nombre = "Pasivos",
                Descripcion = "Obligaciones por pagar",
                Origen = "CREDITO",
                Estado = true
            }
        };

        private static readonly ContabilidadCuenta CUENTA_BANCO = new()
        {
            Id = 2,
            Codigo = "1102-01",
            Nombre = "Banco Cuenta Cheques",
            Descripcion = "Cuenta bancaria para emision de cheques",
            PermiteMovimiento = true,
            Nivel = 1,
            Estado = true,
            Tipo = new ContabilidadTipoCuenta
            {
                Id = 2,
                Nombre = "Activos",
                Descripcion = "Bienes y derechos de la empresa",
                Origen = "DEBITO",
                Estado = true
            }
        };

        // Moneda confirmada desde GET /api/monedas
        // ID 1 = USD, tasa 58.5
        private static readonly ContabilidadMoneda MONEDA_USD = new()
        {
            Id = 1,
            CodigoIso = "USD",
            Nombre = "Dolar",
            Simbolo = "$",
            Descripcion = "Dolar Estadounidense",
            TasaCambio = 58.5m,
            Estado = true,
            FechaCreacion = DateTime.Parse("2026-04-08T23:35:35")
        };

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Converters = { new JavaLocalDateTimeConverter() }
        };

        private static readonly JsonSerializerOptions _readOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ContabilidadService(HttpClient httpClient, ILogger<ContabilidadService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<RespuestaAsiento> EnviarAsientoAsync(DocumentoPorPagar doc)
        {
            try
            {
                var request = ConstruirRequest(doc);
                var json = JsonSerializer.Serialize(request, _jsonOpts);

                _logger.LogInformation("POST /api/asientos -> {Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/asientos", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Contabilidad ({Status}) -> {Body}", response.StatusCode, responseBody);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonSerializer.Deserialize<AsientoContableResponse>(responseBody, _readOpts);

                    if (resultado != null && resultado.Id > 0)
                    {
                        return new RespuestaAsiento
                        {
                            Exitoso = true,
                            IdAsiento = resultado.Id,
                            Mensaje = "Asiento creado exitosamente"
                        };
                    }

                    return new RespuestaAsiento
                    {
                        Exitoso = true,
                        IdAsiento = null,
                        Mensaje = "Asiento registrado pero sin ID en la respuesta"
                    };
                }

                _logger.LogWarning("Error de Contabilidad {Status}: {Body}", response.StatusCode, responseBody);
                return new RespuestaAsiento
                {
                    Exitoso = false,
                    Mensaje = $"Error {(int)response.StatusCode}: {responseBody}"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "No se pudo conectar con Contabilidad");
                return new RespuestaAsiento { Exitoso = false, Mensaje = "No se pudo conectar con el servicio de Contabilidad" };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout con Contabilidad");
                return new RespuestaAsiento { Exitoso = false, Mensaje = "Tiempo de espera agotado al conectar con Contabilidad" };
            }
        }

        private AsientoContableRequest ConstruirRequest(DocumentoPorPagar doc)
        {
            var monto = doc.Monto;
            var fecha = doc.FechaDocumento ?? DateTime.Now;

            return new AsientoContableRequest
            {
                Descripcion = $"Asiento de CxP correspondiente al periodo {fecha:yyyy-MM} - {doc.Proveedor?.Nombre}",
                Auxiliar = new ContabilidadAuxiliar
                {
                    Id = ID_AUXILIAR_CXP,
                    Nombre = NOMBRE_AUXILIAR_CXP,
                    Descripcion = "Modulo de Cuentas por Pagar",
                    Estado = true
                },
                FechaAsiento = fecha.ToString("yyyy-MM-dd"),
                MontoTotal = monto,
                MontoTotalDop = monto * 58.5m,
                TasaCambio = 58.5m,
                Estado = true,
                Moneda = MONEDA_USD,
                Detalles = new List<ContabilidadDetalle>
                {
                    // Debito: Cuentas por Pagar Proveedores (ID 1)
                    new ContabilidadDetalle
                    {
                        Cuenta = CUENTA_CXP_PROVEEDOR,
                        TipoMovimiento = "Debito",
                        Monto = monto
                    },
                    // Credito: Banco Cuenta Cheques (ID 2)
                    new ContabilidadDetalle
                    {
                        Cuenta = CUENTA_BANCO,
                        TipoMovimiento = "Credito",
                        Monto = monto
                    }
                }
            };
        }
    }
}
