using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using GlobalWeather.BusinessLogic;
using GlobalWeather.Controllers;
using GlobalWeather.Dtos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace GlobalWeather.Tests.Controllers
{
    [TestClass]
    public class WeatherControllerTest
    {
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<IHttpClientWrapper> _httpClient;

        private WeatherController _weatherController;

        [TestInitialize]
        public void SetUp()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();  
            _httpClient = new Mock<IHttpClientWrapper>();

            _httpClientFactory.Setup(x => x.CreateHttpClient()).Returns(_httpClient.Object);

            _weatherController = new WeatherController(_httpClientFactory.Object);
        }

        [TestMethod]
        public async Task Index_Must_Load_Countries()
        {
            var countriesList = new List<Country>
            {
                new Country
                {
                    Code = "IN",
                    Name = "India"
                },
                new Country
                {
                    Code = "AU",
                    Name = "Australia"
                }
            };

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(countriesList))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            _httpClient.Setup(x => x.GetAsync("http://localhost:60177/GlobalWeather/Countries")).ReturnsAsync(httpResponseMessage);

            var response = await _weatherController.Index();

            Assert.IsNotNull(response);

            var viewResult = response as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewBag.CountryList);
            Assert.AreEqual(viewResult.ViewBag.CountryList.Count, 2);
            Assert.AreEqual(viewResult.ViewBag.CountryList[0].CountryCode, "IN");
            Assert.AreEqual(viewResult.ViewBag.CountryList[0].CountryName, "India");
            Assert.AreEqual(viewResult.ViewBag.CountryList[1].CountryCode, "AU");
            Assert.AreEqual(viewResult.ViewBag.CountryList[1].CountryName, "Australia");

            _httpClient.Verify(x=>x.GetAsync("http://localhost:60177/GlobalWeather/Countries"), Times.Once);
        }

        [TestMethod]
        public async Task FillCity_Must_Load_Cities()
        {
            var citiesList = new List<City>
            {
                new City
                {
                    Name = "Sydney"
                },
                new City
                {
                    Name = "Perth"
                }
            };

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(citiesList))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            _httpClient.Setup(x => x.GetAsync("http://localhost:60177/GlobalWeather/AU/Cities")).ReturnsAsync(httpResponseMessage);

            var response = await _weatherController.FillCity("AU");

            Assert.IsNotNull(response);

            var viewResult = response as JsonResult;
            Assert.IsNotNull(viewResult);

            dynamic jsonResponseObject = viewResult.Data;

            Assert.IsNotNull(jsonResponseObject);
            Assert.AreEqual(jsonResponseObject.Count, 2);
            Assert.AreEqual(jsonResponseObject[0].CityName, "Sydney");
            Assert.AreEqual(jsonResponseObject[1].CityName, "Perth");

            _httpClient.Verify(x => x.GetAsync("http://localhost:60177/GlobalWeather/AU/Cities"), Times.Once);
        }

        [TestMethod]
        public async Task LoadWeather_Must_Load_Weather()
        {
            var weather = new Weather()
            {
                DewPoint = "London dew point is not available",
                Humidity = "81",
                Location = "London",
                Pressure = "1012",
                SkyConditions = "light intensity drizzle",
                Temperature = "280.32",
                Visibility = "10000",
                Wind = "4.1"
            };

            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(weather))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            _httpClient.Setup(x => x.GetAsync("http://localhost:60177/GlobalWeather/Sydney/Weather")).ReturnsAsync(httpResponseMessage);

            var response = await _weatherController.LoadWeather("Sydney");

            Assert.IsNotNull(response);

            var viewResult = response as JsonResult;
            Assert.IsNotNull(viewResult);

            dynamic jsonResponseObject = viewResult.Data;

            Assert.IsNotNull(jsonResponseObject);
            Assert.AreEqual(jsonResponseObject.DewPoint, "London dew point is not available");
            Assert.AreEqual(jsonResponseObject.Humidity, "81");
            Assert.AreEqual(jsonResponseObject.Location, "London");
            Assert.AreEqual(jsonResponseObject.Pressure, "1012");
            Assert.AreEqual(jsonResponseObject.SkyConditions, "light intensity drizzle");
            Assert.AreEqual(jsonResponseObject.Temperature, "280.32");
            Assert.AreEqual(jsonResponseObject.Visibility, "10000");
            Assert.AreEqual(jsonResponseObject.Wind, "4.1");

            _httpClient.Verify(x => x.GetAsync("http://localhost:60177/GlobalWeather/Sydney/Weather"), Times.Once);
        }
    }
}
