using APIDatosMeteorologicos.Data;
using Microsoft.EntityFrameworkCore;

namespace APIDatosMeteorologicos.Services
{
    public class AlertaService
    {
        private readonly APIDatosMeteorologicosContext _context;
        private readonly ILogger<AlertaService> _logger;

        public AlertaService(APIDatosMeteorologicosContext context, ILogger<AlertaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> VerificarOlaDeCalor()
        {
            try
            {
                // Obtener los últimos 3 registros meteorológicos, en orden descendente por fecha
                var registros = await _context.RegistrosMeteorologicos
                    .OrderByDescending(r => r.Fecha)
                    .Take(3)
                    .ToListAsync();

                registros.Reverse();

                // Verificar si hay 3 registros, si la temperatura supera los 30°C en todos ellos,
                // y si las fechas son consecutivas
                if (registros.Count == 3 &&
                    registros.All(r => r.Temperatura > 30) &&
                    registros[0].Fecha.AddDays(1) == registros[1].Fecha &&
                    registros[1].Fecha.AddDays(1) == registros[2].Fecha)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar ola de calor");
                throw;
            }
        }

        public async Task<bool> VerificarCondicionesVentosas()
        {
            try
            {
                // Obtener los últimos 7 registros meteorológicos, en orden descendente por fecha
                var registros = await _context.RegistrosMeteorologicos
                    .OrderByDescending(r => r.Fecha)
                    .Take(7)
                    .ToListAsync();
                
                registros.Reverse();

                // Verificar si hay 7 registros y si son de días consecutivos
                if (registros.Count == 7 &&
                    registros.Select(r => r.Fecha).Distinct().Count() == 7)
                {
                    // Calcular la velocidad promedio del viento
                    var velocidadPromedio = registros.Average(r => r.VelocidadViento);

                    // Verificar si la velocidad promedio supera los 60 km/h
                    if (velocidadPromedio > 60)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar condiciones ventosas");
                throw;
            }
        }

        public async Task<bool> VerificarRiesgoInundaciones()
        {
            try
            {
                // Obtener los últimos 7 registros meteorológicos, en orden descendente por fecha
                var registros = await _context.RegistrosMeteorologicos
                    .OrderByDescending(r => r.Fecha)
                    .Take(7)
                    .ToListAsync();

                registros.Reverse();

                // Verificar si hay 7 registros y si son de días consecutivos
                if (registros.Count == 7 &&
                    registros.Select(r => r.Fecha).Distinct().Count() == 7)
                {
                    // Calcular la precipitación total en el periodo
                    var precipitacionTotal = registros.Sum(r => r.Precipitacion);

                    // Verificar si la precipitación total supera los 50mm
                    if (precipitacionTotal > 50)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar riesgo de inundaciones");
                throw;
            }
        }

        public async Task<List<string>> VerificarAlertas()
        {
            try
            {
                var alertas = new List<string>();

                if (await VerificarOlaDeCalor())
                {
                    alertas.Add("Ola de Calor");
                }
                if (await VerificarCondicionesVentosas())
                {
                    alertas.Add("Condiciones Ventosas");
                }
                if (await VerificarRiesgoInundaciones())
                {
                    alertas.Add("Alto Riesgo de Inundaciones");
                }

                return alertas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar alertas.");
                throw;
            }
        }
    }
}
