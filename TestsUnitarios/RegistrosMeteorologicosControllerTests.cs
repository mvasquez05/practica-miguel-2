using APIDatosMeteorologicos.Controllers;
using APIDatosMeteorologicos.Data;
using APIDatosMeteorologicos.Models;
using APIDatosMeteorologicos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace TestsUnitarios
{
    public class RegistrosMeteorologicosControllerTests
    {
        private APIDatosMeteorologicosContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<APIDatosMeteorologicosContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockLogger = new Mock<ILogger<APIDatosMeteorologicosContext>>();
            return new APIDatosMeteorologicosContext(options, mockLogger.Object);
        }

        private RegistrosMeteorologicosController CreateController(APIDatosMeteorologicosContext context)
        {
            var mockLogger = new Mock<ILogger<RegistrosMeteorologicosController>>();
            var alertaService = new AlertaService(context, new Mock<ILogger<AlertaService>>().Object);
            var cargaDatosService = new CargaDatosService(context, new Mock<ILogger<CargaDatosService>>().Object);
            return new RegistrosMeteorologicosController(alertaService, cargaDatosService, mockLogger.Object);
        }

        // Test para verificar alertas
        [Fact]
        public async Task VerificarAlertas_DebeRetornarOkConAlertas()
        {
            // Arrange
            using var context = CreateDbContext();
            var controller = CreateController(context);

            // Datos de prueba
            context.RegistrosMeteorologicos.AddRange(
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-2), Temperatura = 31, Humedad = 45, VelocidadViento = 20, Precipitacion = 0 },
                new RegistroMeteorologico { Fecha = DateTime.Today.AddDays(-1), Temperatura = 32, Humedad = 60, VelocidadViento = 22, Precipitacion = 0 },
                new RegistroMeteorologico { Fecha = DateTime.Today, Temperatura = 33, Humedad = 50, VelocidadViento = 25, Precipitacion = 0 }
                // Agregar más registros si se desea activar diferentes alertas
            );

            context.SaveChanges();

            // Act
            var result = await controller.VerificarAlertas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // Verifica que la respuesta contenga las alertas esperadas
        }


        // Test para cargar datos desde JSON
        [Fact]
        public async Task CargarDatosDesdeJson_DebeRetornarOkCuandoCargaArrayJson()
        {
            // Arrange
            using var context = CreateDbContext();
            var controller = CreateController(context);

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
            var result = await controller.CargarDatosDesdeJson(jsonContent);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }


        [Fact]
        public async Task CargarDatosDesdeJson_DebeRetornarOkCuandoCargaObjetoJson()
        {
            // Arrange
            using var context = CreateDbContext();
            var controller = CreateController(context);

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
            var result = await controller.CargarDatosDesdeJson(jsonContent);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }


        // Test para cargar datos desde XML
        [Fact]
        public async Task CargarDatosDesdeXml_DebeRetornarOkCuandoCargaArrayXml()
        {
            // Arrange
            using var context = CreateDbContext();
            var controller = CreateController(context);

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
            var result = await controller.CargarDatosDesdeXml(xmlContent);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

        

        [Fact]
        public async Task CargarDatosDesdeXml_DebeRetornarOkCuandoCargaRegistroXml()
        {
            // Arrange
            using var context = CreateDbContext();
            var controller = CreateController(context);
            
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
            var result = await controller.CargarDatosDesdeXml(xmlContent);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, await context.RegistrosMeteorologicos.CountAsync());
        }

    }
}
