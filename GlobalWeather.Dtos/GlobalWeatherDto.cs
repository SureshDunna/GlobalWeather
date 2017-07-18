using System;
using System.ComponentModel.DataAnnotations;

namespace GlobalWeather.Dtos
{
    public class Country
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class City
    {
        [Required]
        public string Name { get; set; }
    }

    public class Weather
    {
        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public string Wind { get; set; }

        [Required]
        public string Visibility { get; set; }

        [Required]
        public string SkyConditions { get; set; }

        [Required]
        public string Temperature { get; set; }

        [Required]
        public string DewPoint { get; set; }

        [Required]
        public string Humidity { get; set; }

        [Required]
        public string Pressure { get; set; }
    }
}