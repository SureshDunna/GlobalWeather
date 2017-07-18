using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalWeather.Dtos;
using GlobalWeatherApi.BusinessLogic;
using GlobalWeatherApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Weather = GlobalWeather.Dtos.Weather;

namespace GlobalWeatherApi.Tests.Controllers
{
    [TestClass]
    public class WeatherControllerTest
    {
        private Mock<IWeatherServiceClient> _weatherServiceClient;

        private WeatherController _weatherController;

        [TestInitialize]
        public void SetUp()
        {
            _weatherServiceClient = new Mock<IWeatherServiceClient>();
            _weatherController = new WeatherController(_weatherServiceClient.Object);
        }

        [TestMethod]
        public async Task GetCountries_Must_Return_Success()
        {
            _weatherServiceClient.Setup(x => x.GetCountries()).ReturnsAsync(new List<Country>());

             await _weatherController.GetCountries();

            _weatherServiceClient.Verify(x=>x.GetCountries(), Times.Once);
        }

        [TestMethod]
        public async Task GetCities_Must_Return_Success()
        {
            const string countryName = "Australia";
            _weatherServiceClient.Setup(x => x.GetCityByCountryName(countryName)).ReturnsAsync(new List<City>());

            await _weatherController.GetCities(countryName);

            _weatherServiceClient.Verify(x => x.GetCityByCountryName(countryName), Times.Once);
        }

        [TestMethod]
        public async Task GetWeatherByCity_Must_Return_Success()
        {
            const string cityName = "Sydney";
            _weatherServiceClient.Setup(x => x.GetWeatherByCityName(cityName)).ReturnsAsync(new Weather());

            await _weatherController.GetWeatherByCity(cityName);

            _weatherServiceClient.Verify(x => x.GetWeatherByCityName(cityName), Times.Once);
        }
    }
}
