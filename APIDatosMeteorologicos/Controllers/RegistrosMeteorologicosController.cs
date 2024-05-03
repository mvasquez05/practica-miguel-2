using APIDatosMeteorologicos.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace APIDatosMeteorologicos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrosMeteorologicosController : ControllerBase
    {
        private readonly AlertaService _alertaService;
        private readonly CargaDatosService _cargaDatosService;
        private readonly ILogger<RegistrosMeteorologicosController> _logger;

        public RegistrosMeteorologicosController(AlertaService alertaService, CargaDatosService cargaDatosService, ILogger<RegistrosMeteorologicosController> logger)
        {
            _alertaService = alertaService;
            _cargaDatosService = cargaDatosService;
            _logger = logger;
        }

        [HttpGet("verificar-alertas")]
        public async Task<IActionResult> VerificarAlertas()
        {
            try
            {
                var alertas = await _alertaService.VerificarAlertas();
                return Ok(new { Alertas = alertas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar alertas.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("cargar-json"), Consumes("application/json")]
        public async Task<IActionResult> CargarDatosDesdeJson([FromBody] JToken jsonContent)
        {
            try
            {
                await _cargaDatosService.CargarDatosDesdeJson(jsonContent);
                return Ok(new { message = "Datos JSON se cargaron y guardaron correctamente." });
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Error de serialización JSON.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al cargar datos desde JSON.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("cargar-xml"), Consumes("application/xml")]
        public async Task<IActionResult> CargarDatosDesdeXml([FromBody] XElement xmlElement)
        {
            try
            {
                await _cargaDatosService.CargarDatosDesdeXml(xmlElement);
                return Ok(new { message = "Datos XML se cargaron y guardaron correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al cargar datos desde XML.");
                return BadRequest(new { message = "Error al procesar los datos XML.", exception = ex.Message });
            }
        }
    }
}
