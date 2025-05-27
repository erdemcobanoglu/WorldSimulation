import React, { useEffect, useState } from "react";
import "../MapViewer.css";

const weatherSymbols = {
    Sunny: "🌞",
    Cloudy: "☁️",
    Rainy: "🌧",
    Stormy: "🌩",
    Snowy: "❄️",
    Unknown: "❔"
};

const oceanEventSymbols = {
    Tide: "🌊",
    OceanStorm: "🌪",
    Current: "🔄",
    Tsunami: "🌋"
};

const formatTerrain = (terrain) => {
    if (!terrain || typeof terrain !== "string") return "Unknown";

    // Haritaya uygun terrain sınıflarını normalize et
    const normalized = terrain.trim().toLowerCase();

    switch (normalized) {
        case "land":
            return "Land";
        case "sea":
            return "Sea";
        case "air":
            return "Air";
        case "mountain":
            return "Mountain";
        case "desert":
            return "Desert";
        case "ice":
            return "Ice";
        case "island":
            return "Island";
        default:
            return "Unknown";
    }
};


const MapViewer = () => {
    const [tiles, setTiles] = useState([]);
    const [width, setWidth] = useState(0);
    const [height, setHeight] = useState(0);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const getWeatherSymbol = (weather) => weatherSymbols[weather] || "❔";
    const getOceanSymbol = (event) => oceanEventSymbols[event] || "⚠️";

    const renderGrid = () => {
        const grid = [];

        for (let y = 0; y < height; y++) {
            const row = [];
            for (let x = 0; x < width; x++) {
                const tile = tiles.find((t) => t.x === x && t.y === y);

                const content = tile
                    ? tile.oceanEvent
                        ? getOceanSymbol(tile.oceanEvent)
                        : getWeatherSymbol(tile.weather)
                    : "❌";

                const terrainClass = tile ? formatTerrain(tile.terrain) : "Unknown";
                const tileClass = `tile ${terrainClass} ${tile?.oceanEvent ? "ocean" : ""}`;

                row.push(
                    <div key={`${x}-${y}`} className={tileClass}>
                        {content}
                    </div>
                );
            }

            grid.push(
                <div key={y} className="row">
                    {row}
                </div>
            );
        }

        return grid;
    };

    const loadSnapshot = () => {
        setLoading(true);
        fetch("https://localhost:7260/api/map/weather-snapshot")
            .then((res) => {
                if (!res.ok) throw new Error(`HTTP ${res.status}`);
                return res.json();
            })
            .then((data) => {
                const maxX = Math.max(...data.tiles.map((t) => t.x));
                const maxY = Math.max(...data.tiles.map((t) => t.y));
                setTiles(data.tiles);
                setWidth(maxX + 1);
                setHeight(maxY + 1);
                setError(null);
            })
            .catch((err) => {
                console.error("Snapshot hatası:", err);
                setError(err.message);
            })
            .finally(() => setLoading(false));
    };

    // Sayfa ilk açıldığında haritayı oluştur ve snapshot'ı başlat
    useEffect(() => {
        fetch("https://localhost:7260/api/map/generate")
            .then((res) => {
                if (!res.ok) throw new Error(`Harita oluşturulamadı: ${res.status}`);
                return res.json();
            })
            .then(() => {
                loadSnapshot(); // İlk snapshot
                const interval = setInterval(() => {
                    loadSnapshot(); // Her 5 saniyede bir güncelle
                }, 5000);
                return () => clearInterval(interval);
            })
            .catch((err) => {
                console.error("İlk yükleme hatası:", err);
                setError(err.message);
            });
    }, []);

    return (
        <div className="map">
            {/*<h2>Hava Durumu Haritası</h2>*/}
            {loading && <p>Yükleniyor...</p>}
            {error && <p style={{ color: "red" }}>Hata: {error}</p>}
            {!loading && !error && renderGrid()}
        </div>
    );
};

export default MapViewer;
