using APIDatosMeteorologicos.Data;
using APIDatosMeteorologicos.Models;
using APIDatosMeteorologicos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestsUnitarios
{
    public class AlertaServiceTests
    {        
        private APIDatosMeteorologicosContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<APIDatosMeteorologicosContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // instancia de la base de datos
                .Options;

            // Crea un mock del logger
            var mockLogger = new Mock<ILogger<APIDatosMeteorologicosContext>>();

            // Usa el mock del logger al crear la instancia del contexto
            var dbContext = new APIDatosMeteorologicosContext(options, mockLogger.Object);
            return dbContext;
        }

        private AlertaService CreateAlertaService(APIDatosMeteorologicosContext context)
        {
            var mockLogger = new Mock<ILogger<AlertaService>>();
            return new AlertaService(context, mockLogger.Object);
        }

        [Fact]
        public async Task VerificarOlaDeCalor_DebeRetornarTrue_CuandoHayTresDiasConsecutivosConAltaTemperatura()
        {
            // Arrange
            using var context = CreateDbContext();
            var alertaService = CreateAlertaService(context);

            context.RegistrosMeteorologicos.AddRange(
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-2), Temperatura = 31 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-1), Temperatura = 32 },
                new RegistroMeteorologico { Fecha = DateTime.Today, Temperatura = 33 }
            );

            context.SaveChanges();

            // Act
            var resultado = await alertaService.VerificarOlaDeCalor();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task VerificarCondicionesVentosas_DebeRetornarTrue_CuandoVelocidadVientoPromedioSupera60()
        {
            // Arrange
            using var context = CreateDbContext();
            var alertaService = CreateAlertaService(context);

            context.RegistrosMeteorologicos.AddRange(            
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-6), VelocidadViento = 65 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-5), VelocidadViento = 70 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-4), VelocidadViento = 62 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-3), VelocidadViento = 68 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-2), VelocidadViento = 71 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-1), VelocidadViento = 65 },
                new RegistroMeteorologico { Fecha = DateTime.Today, VelocidadViento = 67 }
            );

            context.SaveChanges();

            // Act
            var resultado = await alertaService.VerificarCondicionesVentosas();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task VerificarRiesgoInundaciones_DebeRetornarTrue_CuandoPrecipitacionTotalSupera50mm()
        {
            // Arrange
            using var context = CreateDbContext();
            var alertaService = CreateAlertaService(context);

            context.RegistrosMeteorologicos.AddRange(            
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-6), Precipitacion = 10 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-5), Precipitacion = 12 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-4), Precipitacion = 15 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-3), Precipitacion = 8 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-2), Precipitacion = 5 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-1), Precipitacion = 3 },
                new RegistroMeteorologico { Fecha = DateTime.Today, Precipitacion = 7 } // Asegúrate de que la suma supere los 50mm para que el test sea válido
            );

            context.SaveChanges();

            // Act
            var resultado = await alertaService.VerificarRiesgoInundaciones();

            // Assert
            Assert.True(resultado);
        }
    }
}
