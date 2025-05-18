
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Application.WorldMapService;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;


Console.OutputEncoding = System.Text.Encoding.UTF8;

IWorldMapService mapService = new WorldMapService();
WorldMap map = mapService.CreateMap(30, 10); // 30x10 boyutunda bir harita

PrintMap(map);

Console.WriteLine("\nSimülasyon tamamlandı. Çıkmak için bir tuşa basın...");
Console.ReadKey();


static void PrintMap(WorldMap map)
{
    for (int y = 0; y < map.Height; y++)
    {
        for (int x = 0; x < map.Width; x++)
        {
            Tile tile = map.Tiles[x, y];
            string symbol = tile.Terrain switch
            {
                TerrainType.Land => "L",
                TerrainType.Sea => "S",
                TerrainType.Air => "A",
                _ => "?"
            };
            Console.Write(symbol);
        }
        Console.WriteLine();
    }
}