using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GlobalWeather.Dtos;
using GlobalWeatherApi.BusinessLogic;
using Weather = GlobalWeather.Dtos.Weather;

namespace GlobalWeatherApi.Controllers
{
    [RoutePrefix("GlobalWeather")]
    public class WeatherController : ApiController
    {
        private readonly IWeatherServiceClient _weatherServiceClient;

        public WeatherController(IWeatherServiceClient weatherServiceClient)
        {
            _weatherServiceClient = weatherServiceClient;
        }

        [Route("Countries")]
        [HttpGet]
        [ResponseType(typeof(List<Country>))]
        public async Task<IHttpActionResult> GetCountries()
        {
            return Ok(await _weatherServiceClient.GetCountries());
        }

        [Route("{countryName}/Cities")]
        [HttpGet]
        [ResponseType(typeof(List<City>))]
        public async Task<IHttpActionResult> GetCities(string countryName)
        {
            return Ok(await _weatherServiceClient.GetCityByCountryName(countryName));
        }

        [Route("{cityName}/Weather")]
        [HttpGet]
        [ResponseType(typeof(Weather))]
        public async Task<IHttpActionResult> GetWeatherByCity(string cityName)
        {
            return Ok(await _weatherServiceClient.GetWeatherByCityName(cityName));
        }
    }
}
