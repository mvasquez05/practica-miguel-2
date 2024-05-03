using APIDatosMeteorologicos.Data;
using APIDatosMeteorologicos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace APIDatosMeteorologicos.Services
{
    public class CargaDatosService
    {
        private readonly APIDatosMeteorologicosContext _context;
        private readonly ILogger<CargaDatosService> _logger;

        public CargaDatosService(APIDatosMeteorologicosContext context, ILogger<CargaDatosService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ActualizarRegistroSiExisteFechaDuplicada(RegistroMeteorologico registro)
        {
            // Aqui verificamos si el nuevo registro enviado al API, tiene fecha repetida con alguno de los registros que ya están almacenados
            // en la base de datos. En caso de encontrarse fecha repetida, procedemos a actualizar el registro de la base de datos con los datos
            // del nuevo registro entrante. Agregué esto en caso de que se requieran actualizar los datos de un registro previamente introducido

            // Buscar un registro existente en la base de datos que tenga la misma fecha
            var registroExistente = await _context.RegistrosMeteorologicos
                .FirstOrDefaultAsync(r => r.Fecha == registro.Fecha);
            
            // Si se encuentra un registro existente, actualizarlo
            if (registroExistente != null)
            {
                // Actualizar las propiedades del registro existente con los datos del nuevo registro
                registroExistente.Temperatura = registro.Temperatura;
                registroExistente.Humedad = registro.Humedad;
                registroExistente.VelocidadViento = registro.VelocidadViento;
                registroExistente.Precipitacion = registro.Precipitacion;

                // Marcar la instancia existente como actualizada
                _context.Entry(registroExistente).State = EntityState.Modified;

                // Retornar false porque se encontró y actualizó un registro existente
                return false;
            }

            // Retornar true si no se encontró un registro existente con la misma fecha
            return true;
        }


        public async Task<bool> ActualizarRegistrosSiExistenFechasDuplicadas(List<RegistroMeteorologico> registros)
        {
            // Verificamos si uno o más registros de los enviados al API como Array, tienen fechas repetidas con algunos de los registros ya almacenados
            // en la base de datos. En caso de encontrarse fechas repetidas, procedemos a actualizar los registros de la base de datos.
            // Agregué esto en caso de que se requiera actualizar los datos de uno o varios registros previamente introducidos

            // Iterar sobre una copia de la lista para poder modificar la original durante la iteración
            foreach (var registro in registros.ToList())
            {
                // Buscar un registro existente en la base de datos que tenga la misma fecha
                var registroExistente = await _context.RegistrosMeteorologicos
                    .FirstOrDefaultAsync(r => r.Fecha == registro.Fecha);

                // Si se encuentra un registro existente, actualizarlo
                if (registroExistente != null)
                {
                    // Actualizar las propiedades del registro existente con los datos del nuevo registro
                    registroExistente.Temperatura = registro.Temperatura;
                    registroExistente.Humedad = registro.Humedad;
                    registroExistente.VelocidadViento = registro.VelocidadViento;
                    registroExistente.Precipitacion = registro.Precipitacion;

                    // Marcar la instancia existente como actualizada
                    _context.Entry(registroExistente).State = EntityState.Modified;

                    // Eliminar el registro duplicado de la lista proporcionada
                    registros.Remove(registro);
                }
            }
            
            // Retornar true si quedan registros, false si la lista está vacía
            return registros.Any();
        }




        public async Task CargarDatosDesdeJson(JToken jsonContent)
        {
            try
            {
                if (jsonContent.Type == JTokenType.Array)
                {
                    List<RegistroMeteorologico> registros = jsonContent.ToObject<List<RegistroMeteorologico>>();

                    if (await ActualizarRegistrosSiExistenFechasDuplicadas(registros))
                    {

                        _context.RegistrosMeteorologicos.AddRange(registros);
                    }
                }

                else if (jsonContent.Type == JTokenType.Object)
                {
                    RegistroMeteorologico registro = jsonContent.ToObject<RegistroMeteorologico>();

                    if (await ActualizarRegistroSiExisteFechaDuplicada(registro))
                    {
                     
                        _context.RegistrosMeteorologicos.Add(registro);
                    }
                }
                else
                {
                    throw new JsonSerializationException("El contenido JSON no es un objeto ni un array.");
                }

                await _context.SaveChangesAsync();
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Error de serialización JSON al cargar datos.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al cargar datos desde JSON.");
                throw;
            }
        }

        public async Task CargarDatosDesdeXml(XElement xmlElement)
        {
            try
            {
                if (xmlElement.Name.LocalName == "ArrayOfRegistroMeteorologico")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ListaRegistrosMeteorologicos));
                    ListaRegistrosMeteorologicos registros = (ListaRegistrosMeteorologicos)serializer.Deserialize(xmlElement.CreateReader());
                    _context.RegistrosMeteorologicos.AddRange(registros.Registros);
                }

                else if (xmlElement.Name.LocalName == "RegistroMeteorologico")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegistroMeteorologico));
                    RegistroMeteorologico registro = (RegistroMeteorologico)serializer.Deserialize(xmlElement.CreateReader());
                    _context.RegistrosMeteorologicos.Add(registro);
                }
                else
                {
                    throw new InvalidOperationException("El elemento XML raíz no es reconocido.");
                }

                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de operación al cargar datos desde XML.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al cargar datos desde XML.");
                throw;
            }
        }
    }
}
