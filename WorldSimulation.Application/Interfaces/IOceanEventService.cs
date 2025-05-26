using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Entities.Event;

namespace WorldSimulation.Application.Interfaces
{
    public interface IOceanEventService
    {
        void Update(DateTime currentTime);
        IReadOnlyList<OceanEvent> GetActiveEvents();
    }
}

