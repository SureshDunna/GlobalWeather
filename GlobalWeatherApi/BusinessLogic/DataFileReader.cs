using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlobalWeatherApi.BusinessLogic
{
    public interface IDataFileReader
    {
        Task<T> DeserializeObjectJsonDataFile<T>(string fileNamePath);
    }

    public class DataFileReader : IDataFileReader
    {
        public Task<T> DeserializeObjectJsonDataFile<T>(string fileNamePath)
        {
            return Task.Run(() =>
            {
                if(string.IsNullOrEmpty(fileNamePath)) throw new InvalidDataException();
                if(!File.Exists(fileNamePath)) throw new FileNotFoundException($"{fileNamePath} not found");

                var jsonString = File.ReadAllText(fileNamePath);
                return JsonConvert.DeserializeObject<T>(jsonString);
            });
        }
    }
}