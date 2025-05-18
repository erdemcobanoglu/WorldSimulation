using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Service
{
    public class WeatherService : IWeatherService
    {
        public WeatherType GetCurrentWeather(int tick)
        {
            // Döngüsel olarak hava durumu değişimi örneği (her 10 tick'te değişir)
            int phase = tick % 50;

            return phase switch
            {
                <= 10 => WeatherType.Sunny,
                <= 20 => WeatherType.Cloudy,
                <= 30 => WeatherType.Rainy,
                <= 40 => WeatherType.Stormy,
                _ => WeatherType.Snowy
            };
        }
    }
}
