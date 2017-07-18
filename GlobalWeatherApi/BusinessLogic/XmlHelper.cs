using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GlobalWeatherApi.BusinessLogic
{
    public interface IXmlHelper
    {
        Task<T> DeserializeRoot<T>(string serializedValue);
    }

    public class XmlHelper : IXmlHelper
    {
        public Task<T> DeserializeRoot<T>(string serializedValue)
        {
            if(string.IsNullOrWhiteSpace(serializedValue)) throw new InvalidDataException();

            var xDocument = XDocument.Parse(serializedValue);
            if (xDocument.Root == null) throw new InvalidDataException();

            return Task.Run(() =>
            {
                var rootValue = xDocument.Root.Value;

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(rootValue)))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T) serializer.Deserialize(stream);
                }
            });
        }
    }
}