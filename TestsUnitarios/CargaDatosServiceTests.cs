using APIDatosMeteorologicos.Data;
using APIDatosMeteorologicos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace TestsUnitarios
{
    public class CargaDatosServiceTests
    {       
        private APIDatosMeteorologicosContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<APIDatosMeteorologicosContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
                        
            // Crea un mock del logger
            var mockLogger = new Mock<ILogger<APIDatosMeteorologicosContext>>();

            // Usa el mock del logger al crear la instancia del contexto
            var dbContext = new APIDatosMeteorologicosContext(options, mockLogger.Object);
            return dbContext;
        }

        private CargaDatosService CreateCargaDatosService(APIDatosMeteorologicosContext context)
        {
            var mockLogger = new Mock<ILogger<CargaDatosService>>();
            return new CargaDatosService(context, mockLogger.Object);
        }

        [Fact]
        public async Task CargarDatosDesdeJson_DebeAgregarRegistros_CuandoRecibeArrayJson()
        {
            // Arrange
            using var context = CreateDbContext();
            var cargaDatosService = CreateCargaDatosService(context);

            var jsonContent = JToken.Parse(
            @"[
                {
                    ""fecha"": ""2023-11-30"",
                    ""temperatura"": 25,
                    ""humedad"": 60,
                    ""velocidadViento"": 10,
                    ""precipitacion"": 2
                }
            ]"
            );
            
            // Act
            await cargaDatosService.CargarDatosDesdeJson(jsonContent);

            // Assert
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

        [Fact]
        public async Task CargarDatosDesdeJson_DebeAgregarRegistro_CuandoRecibeObjetoJson()
        {
            // Arrange
            using var context = CreateDbContext();
            var cargaDatosService = CreateCargaDatosService(context);

            var jsonContent = JToken.Parse(
            @"{
                ""fecha"": ""2023-11-30"",
                ""temperatura"": 25,
                ""humedad"": 60,
                ""velocidadViento"": 10,
                ""precipitacion"": 2
            }"
            );

            // Act
            await cargaDatosService.CargarDatosDesdeJson(jsonContent);

            // Assert
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

        [Fact]
        public async Task CargarDatosDesdeXml_DebeAgregarRegistros_CuandoRecibeArrayXml()
        {
            // Arrange
            using var context = CreateDbContext();
            var cargaDatosService = CreateCargaDatosService(context);

            var xmlContent = XElement.Parse(
            @"<ArrayOfRegistroMeteorologico>
                <RegistroMeteorologico>
                    <fecha>2023-11-30</fecha>
                    <temperatura>25</temperatura>
                    <humedad>60</humedad>
                    <velocidadViento>10</velocidadViento>
                    <precipitacion>2</precipitacion>
                </RegistroMeteorologico>
            </ArrayOfRegistroMeteorologico>"
            );

            // Act
            await cargaDatosService.CargarDatosDesdeXml(xmlContent);

            // Assert
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

        [Fact]
        public async Task CargarDatosDesdeXml_DebeAgregarRegistro_CuandoRecibeRegistroXml()
        {
            // Arrange
            using var context = CreateDbContext();
            var cargaDatosService = CreateCargaDatosService(context);

            var xmlContent = XElement.Parse(
            @"<RegistroMeteorologico>
                <fecha>2023-11-30</fecha>
                <temperatura>25</temperatura>
                <humedad>60</humedad>
                <velocidadViento>10</velocidadViento>
                <precipitacion>2</precipitacion>
            </RegistroMeteorologico>"
            );

            // Act
            await cargaDatosService.CargarDatosDesdeXml(xmlContent);

            // Assert
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

    }
}
