﻿using System.Diagnostics.CodeAnalysis;

namespace GlobalWeatherApi.BusinessLogic
{
    [ExcludeFromCodeCoverage]
    public class Rootobject
    {
        public Coord Coord { get; set; }
        public Weather[] Weather { get; set; }
        public string Base { get; set; }
        public Main Main { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public int Dt { get; set; }
        public Sys Sys { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cod { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Coord
    {
        public float Lon { get; set; }
        public float Lat { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Main
    {
        public float Temp { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public float TempMin { get; set; }
        public float TempMax { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Wind
    {
        public float Speed { get; set; }
        public int Deg { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Clouds
    {
        public int All { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Sys
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public float Message { get; set; }
        public string Country { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}