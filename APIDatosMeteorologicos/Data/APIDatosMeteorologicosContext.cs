using APIDatosMeteorologicos.Models;
using Microsoft.EntityFrameworkCore;

namespace APIDatosMeteorologicos.Data
{
    public class APIDatosMeteorologicosContext : DbContext
    {
        private readonly ILogger<APIDatosMeteorologicosContext> _logger;

        public APIDatosMeteorologicosContext(DbContextOptions<APIDatosMeteorologicosContext> options, ILogger<APIDatosMeteorologicosContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public DbSet<RegistroMeteorologico> RegistrosMeteorologicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);
                // Aquí se puede añadir configuración personalizada de los modelos si es necesario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió al crear el modelo.");
                throw; // Lanzar de nuevo la excepción para que no se silencie
            }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió al guardar los cambios en la base de datos.");
                throw; // Lanzar de nuevo la excepción para no silenciarla
            }
        }
    }
}
