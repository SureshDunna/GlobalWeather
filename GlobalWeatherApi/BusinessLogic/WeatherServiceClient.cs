using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GlobalWeather.Dtos;

namespace GlobalWeatherApi.BusinessLogic
{
    public interface IWeatherServiceClient
    {
        Task<List<Country>> GetCountries();
        Task<List<City>> GetCityByCountryName(string countryName);
        Task<GlobalWeather.Dtos.Weather> GetWeatherByCityName(string cityName);
    }

    public class WeatherServiceClient : IWeatherServiceClient
    {
        private readonly string _getCitiesApiUrl;
        private readonly string _getWeatherApiUrl;
        private readonly string _getWeatherApiKey;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDataFileReader _dataFileReader;
        private readonly IXmlHelper _xmlHelper;
        private readonly IExecutionContext _executionContext;

        public WeatherServiceClient(IHttpClientFactory httpClientFactory,
            IDataFileReader dataFileReader,
            IXmlHelper xmlHelper,
            IExecutionContext executionContextex)
        {
            _httpClientFactory = httpClientFactory;
            _dataFileReader = dataFileReader;
            _xmlHelper = xmlHelper;
            _executionContext = executionContextex;

            _getCitiesApiUrl = ConfigurationManager.AppSettings["GetCitiesApiUrl"];
            if (string.IsNullOrEmpty(_getCitiesApiUrl))
                throw new ConfigurationErrorsException("Get Cities Api Url is not configured");

            _getWeatherApiUrl = ConfigurationManager.AppSettings["GetWeatherApiUrl"];
            if (string.IsNullOrEmpty(_getWeatherApiUrl))
                throw new ConfigurationErrorsException("Get Weather Api Url is not configured");

            _getWeatherApiKey = ConfigurationManager.AppSettings["GetWeatherApiKey"];
            if (string.IsNullOrEmpty(_getWeatherApiKey))
                throw new ConfigurationErrorsException("Api key to access Get Weather Api is not configured");
        }

        public async Task<List<Country>> GetCountries()
        {
            return await _dataFileReader.DeserializeObjectJsonDataFile<List<Country>>(Path.Combine(_executionContext.BinDirectory, @"DataFiles\countries.json"));
        }

        public async Task<List<City>> GetCityByCountryName(string countryName)
        {
            using (var httpClient = _httpClientFactory.CreateHttpClient())
            {
                var apiUrl = $"{_getCitiesApiUrl}?CountryName={countryName}";

                var response = await httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var apiResponseString = await response.Content.ReadAsStringAsync();

                var apiResponseObject = await _xmlHelper.DeserializeRoot<NewDataSet>(apiResponseString);

                return
                    apiResponseObject?.Table.Select(newDataSetTable => new City {Name = newDataSetTable.City}).ToList();
            }
        }

        public async Task<GlobalWeather.Dtos.Weather> GetWeatherByCityName(string cityName)
        {
            using (var httpClient = _httpClientFactory.CreateHttpClient())
            {
                var apiUrl = $"{_getWeatherApiUrl}?q={cityName}&appid={_getWeatherApiKey}";

                var response = await httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadAsAsync<Rootobject>();

                return new GlobalWeather.Dtos.Weather
                {
                    DewPoint = $"{cityName} dew point is not available",
                    Humidity = apiResponse.Main.Humidity.ToString(),
                    Location = cityName,
                    Pressure = apiResponse.Main.Pressure.ToString(),
                    SkyConditions = apiResponse.Weather[0].Description,
                    Time = DateTime.Now,
                    Temperature = apiResponse.Main.Temp.ToString(CultureInfo.InvariantCulture),
                    Visibility = apiResponse.Visibility.ToString(),
                    Wind = apiResponse.Wind.Speed.ToString(CultureInfo.InvariantCulture)
                };
            }
        }
    }

}