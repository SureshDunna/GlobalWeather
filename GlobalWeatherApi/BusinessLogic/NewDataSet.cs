using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GlobalWeatherApi.BusinessLogic
{
    [ExcludeFromCodeCoverage]
    [XmlRoot("NewDataSet")]
    public class NewDataSet
    {
        [XmlElement("Table")]
        public NewDataSetTable[] Table { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class NewDataSetTable
    {
        public string Country { get; set; }
        public string City { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class GetCitiesByCountryResponse
    {
        public string String { get; set; }
    }
}