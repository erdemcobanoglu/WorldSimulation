using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Interfaces
{
    public interface IWeatherService
    {
        WeatherType GetCurrentWeather(int tick);
        void UpdateWeather(WorldMap map, DateTime currentTime);
    }
}
