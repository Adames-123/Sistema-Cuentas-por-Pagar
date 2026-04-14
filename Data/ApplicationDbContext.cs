using Microsoft.EntityFrameworkCore;
using Sistema_Documentos_por_Pagar.Models;

namespace Sistema_Documentos_por_Pagar.Data
{
    public class ApplicationDbContext : DbContext
    {
        
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Proveedor> Proveedores { get; set; }
            public DbSet<DocumentoPorPagar> DocumentosPagar { get; set; }
            public DbSet<Concepto> ConceptosPago { get; set; }
        }
    }

