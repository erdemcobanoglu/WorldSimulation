using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Entities.Event;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Service
{
    public class OceanEventService : IOceanEventService
    {
        private readonly List<OceanEvent> _activeEvents = new();
        private readonly Random _random = new();

        private readonly WorldMap _map;

        public OceanEventService(WorldMap map)
        {
            _map = map;
        }

        public void Update(DateTime currentTime)
        {
            // Süresi dolmuş olayları bul
            var expiredEvents = _activeEvents
                .Where(ev => (currentTime - ev.StartTime).TotalMinutes > ev.Duration)
                .ToList();

            // Bu olayların Tile üzerindeki etkisini temizle
            foreach (var expired in expiredEvents)
            {
                expired.Location.CurrentOceanEvent = null;
            }

            // Şimdi olay listesinden sil
            _activeEvents.RemoveAll(ev =>
                (currentTime - ev.StartTime).TotalMinutes > ev.Duration);

            // Etkileri uygula (hala aktif olanlar)
            foreach (var oceanEvent in _activeEvents)
            {
                ApplyEventEffect(oceanEvent);
            }

            // Yeni olay tetikle
            if (ShouldTriggerNewEvent())
            {
                var newEvent = GenerateRandomOceanEvent(currentTime);

                if (newEvent != null)
                {
                    _activeEvents.Add(newEvent);
                    ApplyEventEffect(newEvent);
                }
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
            // Water hücrelerini filtrele
            var waterTiles = new List<Tile>();

            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    var tile = _map.Tiles[x, y];
                    if (tile.Terrain == TerrainType.Sea)
                    {
                        waterTiles.Add(tile);
                    }
                }
            }

            if (waterTiles.Count == 0)
            {
                return null; // Okyanus olayı oluşturulamaz
            }

            // Rasgele bir water tile seç
            var selectedTile = waterTiles[_random.Next(waterTiles.Count)];

            var eventType = (OceanEventType)_random.Next(0, Enum.GetValues(typeof(OceanEventType)).Length);
            var duration = _random.Next(5, 15);
            var intensity = _random.NextDouble() * 10;

            return new OceanEvent
            {
                EventType = eventType,
                Location = selectedTile,
                Duration = duration,
                Intensity = intensity,
                StartTime = currentTime
            };
        }

        private void ApplyEventEffect(OceanEvent oceanEvent)
        {
            // Burada olayın çevresel etkilerini uygula
            // Örn: canlıları etkileyen bir metod çağırılabilir.

            var tile = oceanEvent.Location;

            // Mevcut etkisini yaz (mantıksal işaretleme)
            tile.CurrentOceanEvent = oceanEvent.EventType;
        }
    }

}
