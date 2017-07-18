using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GlobalWeather.Dtos;
using GlobalWeatherApi.BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GlobalWeatherApi.Tests.BusinessLogic
{
    [TestClass]
    public class WeatherServiceClientTest
    {
        private Mock<IDataFileReader> _dataFileReader;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<IXmlHelper> _xmlHelper;
        private Mock<IExecutionContext> _executionContext;

        private IWeatherServiceClient _weatherServiceClient;

        [TestInitialize]
        public void SetUp()
        {
            _dataFileReader = new Mock<IDataFileReader>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _xmlHelper = new Mock<IXmlHelper>();
            _executionContext = new Mock<IExecutionContext>();

            ConfigurationManager.AppSettings["GetCitiesApiUrl"] = "http://localhost/GetCities";
            ConfigurationManager.AppSettings["GetWeatherApiUrl"] = "http://localhost/GetWeather";
            ConfigurationManager.AppSettings["GetWeatherApiKey"] = "abcd1234";

            _weatherServiceClient = new WeatherServiceClient(_httpClientFactory.Object,
                _dataFileReader.Object,
                _xmlHelper.Object,
                _executionContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WeatherServiceClient_Must_Throw_Exception_If_GetCitiesApiUrl_Not_Configured()
        {
            ConfigurationManager.AppSettings.Clear();
            var weatherServiceClient = new WeatherServiceClient(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WeatherServiceClient_Must_Throw_Exception_If_GetWeatherApiUrl_Not_Configured()
        {
            ConfigurationManager.AppSettings.Clear();
            ConfigurationManager.AppSettings["GetCitiesApiUrl"] = "http://localhost/GetCities";
            var weatherServiceClient = new WeatherServiceClient(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WeatherServiceClient_Must_Throw_Exception_If_GetWeatherApiKey_Not_Configured()
        {
            ConfigurationManager.AppSettings.Clear();
            ConfigurationManager.AppSettings["GetCitiesApiUrl"] = "http://localhost/GetCities";
            ConfigurationManager.AppSettings["GetWeatherApiUrl"] = "http://localhost/GetWeather";

            var weatherServiceClient = new WeatherServiceClient(null, null, null, null);
        }

        [TestMethod]
        public async Task GetCountries_Must_Return_Success_Response()
        {
            var countries = new List<Country>
            {
                new Country
                {
                    Code = "Aus",
                    Name = "Australia"
                },
                new Country
                {
                    Code = "Ind",
                    Name = "India"
                }
            };

            _executionContext.Setup(x => x.BinDirectory).Returns(@"D:\localpath");
            _dataFileReader.Setup(x => x.DeserializeObjectJsonDataFile<List<Country>>(It.IsAny<string>()))
                .ReturnsAsync(countries);

            var response = await _weatherServiceClient.GetCountries();

            Assert.IsNotNull(response);
            Assert.AreSame(response, countries);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetCountries_Must_Throw_Exception_If_DeserializeObjectJsonDataFile_Throws_Exception()
        {
            _executionContext.Setup(x => x.BinDirectory).Returns(@"D:\localpath");
            _dataFileReader.Setup(x => x.DeserializeObjectJsonDataFile<List<Country>>(It.IsAny<string>()))
                .Throws<Exception>();

            await _weatherServiceClient.GetCountries();
        }

        [TestMethod]
        public async Task GetCityByCountryName_Must_Return_Success()
        {
            var httpClient = new Mock<IHttpClientWrapper>();
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("ResponseString")
            };
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            var newDataSet = new NewDataSet {Table = new NewDataSetTable[2]};
            newDataSet.Table[0] = new NewDataSetTable
            {
                Country = "Australia",
                City = "Sydney"
            };
            newDataSet.Table[1] = new NewDataSetTable
            {
                Country = "Australia",
                City = "Melbourne"
            };

            _httpClientFactory.Setup(x => x.CreateHttpClient()).Returns(httpClient.Object);
            _xmlHelper.Setup(x => x.DeserializeRoot<NewDataSet>("ResponseString")).ReturnsAsync(newDataSet);

            var response = await _weatherServiceClient.GetCityByCountryName("Australia");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Count, 2);
            Assert.AreEqual(response[0].Name, "Sydney");
            Assert.AreEqual(response[1].Name, "Melbourne");
        }

        [TestMethod]
        public async Task GetCityByCountryName_Must_Return_Null_If_Invalid_Response_Returned_From_Api()
        {
            var httpClient = new Mock<IHttpClientWrapper>();
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("ResponseString")
            };
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            _httpClientFactory.Setup(x => x.CreateHttpClient()).Returns(httpClient.Object);
            _xmlHelper.Setup(x => x.DeserializeRoot<NewDataSet>("ResponseString")).ReturnsAsync(()=>null);

            var response = await _weatherServiceClient.GetCityByCountryName("Australia");

            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task GetWeatherByCityName_Must_Return_Success()
        {
            var httpClient = new Mock<IHttpClientWrapper>();
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("{\"coord\":{\"lon\":-0.13,\"lat\":51.51},\"weather\":[{\"id\":300,\"main\":\"Drizzle\",\"description\":\"light intensity drizzle\",\"icon\":\"09d\"}],\"base\":\"stations\",\"main\":{\"temp\":280.32,\"pressure\":1012,\"humidity\":81,\"temp_min\":279.15,\"temp_max\":281.15},\"visibility\":10000,\"wind\":{\"speed\":4.1,\"deg\":80},\"clouds\":{\"all\":90},\"dt\":1485789600,\"sys\":{\"type\":1,\"id\":5091,\"message\":0.0103,\"country\":\"GB\",\"sunrise\":1485762037,\"sunset\":1485794875},\"id\":2643743,\"name\":\"London\",\"cod\":200}")
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

            _httpClientFactory.Setup(x => x.CreateHttpClient()).Returns(httpClient.Object);

            var response = await _weatherServiceClient.GetWeatherByCityName("London");

            Assert.IsNotNull(response);
            Assert.AreEqual(response.DewPoint, "London dew point is not available");
            Assert.AreEqual(response.Humidity, "81");
            Assert.AreEqual(response.Location, "London");
            Assert.AreEqual(response.Pressure, "1012");
            Assert.AreEqual(response.SkyConditions, "light intensity drizzle");
            Assert.AreEqual(response.Temperature, "280.32");
            Assert.AreEqual(response.Visibility, "10000");
            Assert.AreEqual(response.Wind, "4.1");
        }
    }
}
