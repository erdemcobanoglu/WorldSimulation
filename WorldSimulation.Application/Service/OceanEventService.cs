using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Service
{
    public class OceanEventService : IOceanEventService
    {
        private readonly List<OceanEvent> _activeEvents = new();
        private readonly Random _random = new();

        public void Update(DateTime currentTime)
        {
            // Süresi dolmuş olayları kaldır
            _activeEvents.RemoveAll(ev =>
                (currentTime - ev.StartTime).TotalMinutes > ev.Duration);

            // Etkileri uygula
            foreach (var oceanEvent in _activeEvents)
            {
                ApplyEventEffect(oceanEvent);
            }

            // Yeni olay tetikle
            if (ShouldTriggerNewEvent())
            {
                var newEvent = GenerateRandomOceanEvent(currentTime);
                _activeEvents.Add(newEvent);
                ApplyEventEffect(newEvent);
            }
        }

        public IReadOnlyList<OceanEvent> GetActiveEvents()
        {
            return _activeEvents.AsReadOnly();
        }

        private bool ShouldTriggerNewEvent()
        {
            return _random.NextDouble() < 0.1; // %10 olasılıkla tetikle
        }

        private OceanEvent GenerateRandomOceanEvent(DateTime currentTime)
        {
            var eventType = (OceanEventType)_random.Next(0, Enum.GetNames(typeof(OceanEventType)).Length);
            var location = new Tile(_random.Next(0, 100), _random.Next(0, 100));
            var duration = _random.Next(5, 15);
            var intensity = _random.NextDouble() * 10;

            return new OceanEvent
            {
                EventType = eventType,
                Location = location,
                Duration = duration,
                Intensity = intensity,
                StartTime = currentTime
            };
        }

        private void ApplyEventEffect(OceanEvent oceanEvent)
        {
            // Burada olayın çevresel etkilerini uygula
            // Örn: canlıları etkileyen bir metod çağırılabilir.
        }
    }

}
