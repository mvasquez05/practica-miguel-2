using System.Xml.Serialization;

namespace APIDatosMeteorologicos.Models
{
    // Esta clase es un contenedor para una lista de registros meteorológicos, y se utiliza para la deserialización de XML.
    // Se añade Namespace = "" para indicar que no se espera un espacio de nombres.    
    [XmlRoot("ArrayOfRegistroMeteorologico", Namespace = "")]
    public class ListaRegistrosMeteorologicos
    {
        [XmlElement("RegistroMeteorologico")]
        public List<RegistroMeteorologico> Registros { get; set; }
    }

    public class RegistroMeteorologico
    {       
        // El Id se puede omitir ya que no se incluye en el XML
        [XmlIgnore]
        public int Id { get; set; }

        [XmlElement("fecha")]
        public DateTime Fecha { get; set; }

        [XmlElement("temperatura")]
        public double Temperatura { get; set; }

        [XmlElement("humedad")]
        public double Humedad { get; set; }

        [XmlElement("velocidadViento")]
        public double VelocidadViento { get; set; }

        [XmlElement("precipitacion")]
        public double Precipitacion { get; set; }
    }
}
