using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Domain.Entities
{
    public class SimulationTickResult
    {
        public int Tick { get; set; }
        public string Weather { get; set; }
        public string WeatherSymbol { get; set; }
        public List<string> MapLines { get; set; } // Her satır bir string (örn: "🟫🟫🌊")
    }
}
