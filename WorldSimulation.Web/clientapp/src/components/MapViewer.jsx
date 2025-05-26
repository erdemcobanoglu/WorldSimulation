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
                    : "🟫";

                const tileClass = `tile${tile?.oceanEvent ? " ocean" : ""}`;

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
                console.error("Veri alınamadı:", err);
                setError(err.message);
            })
            .finally(() => setLoading(false));
    };

    const startSimulation = () => {
        setLoading(true);
        fetch("https://localhost:7260/api/map/generate")
            .then((res) => {
                if (!res.ok) throw new Error(`Simülasyon başlatılamadı: ${res.status}`);
                return res.json();
            })
            .then(() => {
                loadSnapshot(); // Simülasyon sonrası snapshot çek
            })
            .catch((err) => {
                console.error("Simülasyon hatası:", err);
                setError(err.message);
            });
    };

    // ⏱ Snapshot'ı 5 saniyede bir çekmek için interval
    useEffect(() => {
        const interval = setInterval(() => {
            loadSnapshot();
        }, 5000); // her 5 saniyede bir

        return () => clearInterval(interval); // component unmount edilirse temizle
    }, []);

    return (
        <div className="map">
            <h3>Harita Görünümü</h3>
            <button onClick={startSimulation}>Simülasyonu Başlat</button>

            {loading && <p>Yükleniyor...</p>}
            {error && <p style={{ color: "red" }}>Hata: {error}</p>}
            {!loading && !error && renderGrid()}
        </div>
    );
};

export default MapViewer;
