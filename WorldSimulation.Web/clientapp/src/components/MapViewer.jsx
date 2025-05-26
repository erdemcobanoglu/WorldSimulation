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

const MapViewer = () => {
    const [tiles, setTiles] = useState([]);
    const [width, setWidth] = useState(0);
    const [height, setHeight] = useState(0);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch("https://localhost:7260/api/map/weather-snapshot") 
            .then((res) => {
                if (!res.ok) {
                    throw new Error(`HTTP ${res.status}: ${res.statusText}`);
                }
                return res.json();
            })
            .then((data) => {
                if (!data.tiles || data.tiles.length === 0) {
                    throw new Error("Harita verisi boş veya eksik.");
                }

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
            .finally(() => {
                setLoading(false);
            });
    }, []);

    const getSymbol = (weather) => weatherSymbols[weather] || "❔";

    const renderGrid = () => {
        const grid = [];

        for (let y = 0; y < height; y++) {
            const row = [];
            for (let x = 0; x < width; x++) {
                const tile = tiles.find((t) => t.x === x && t.y === y);
                row.push(
                    <div key={`${x}-${y}`} className="tile">
                        {tile ? getSymbol(tile.weather) : "🟫"}
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

    return (
        <div className="map">
            {loading && <p>Yükleniyor...</p>}
            {error && <p style={{ color: "red" }}>Hata: {error}</p>}
            {!loading && !error && renderGrid()}
        </div>
    );
};

export default MapViewer;
