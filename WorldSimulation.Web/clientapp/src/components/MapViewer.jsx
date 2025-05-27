﻿import React, { useEffect, useState } from "react";
import "../MapViewer.css";

const weatherSymbols = {
    Sunny: "🌞",
    Cloudy: "☁️",
    Rainy: "🌧",
    Stormy: "⚩️",
    Snowy: "❄️",
    Unknown: "❔"
};

const oceanEventSymbols = {
    Tide: "🌊",
    OceanStorm: "🌪",
    Current: "🔄",
    Tsunami: "🌋"
};

const terrainIcons = {
    mountain: "⛰️",
    desert: "🏜️",
    island: "🏝️",
    land: "🏘️"
};

const dynamicTerrainMap = {
    Rainy: { Land: "Mud" },
    Snowy: { Land: "Ice" }
};

const formatTerrain = (terrain) => {
    if (!terrain || typeof terrain !== "string") return "Unknown";
    const normalized = terrain.trim().toLowerCase();
    switch (normalized) {
        case "land": return "Land";
        case "sea": return "Sea";
        case "air": return "Air";
        case "mountain": return "Mountain";
        case "desert": return "Desert";
        case "ice": return "Ice";
        case "island": return "Island";
        default: return "Unknown";
    }
};

const applyWeatherEffect = (terrain, weather) => {
    const weatherEffects = dynamicTerrainMap[weather];
    return weatherEffects && weatherEffects[terrain] ? weatherEffects[terrain] : terrain;
};

const getTimePhase = (hour) => {
    if (hour >= 5 && hour < 8) return "dawn";
    if (hour >= 8 && hour < 17) return "day";
    if (hour >= 17 && hour < 20) return "dusk";
    return "night";
};

const getLocalHour = (globalHour, tileX, width) => {
    const offset = Math.floor((tileX / width) * 24);
    return (globalHour + offset) % 24;
};

const getTileBrightness = (hour) => {
    if (hour >= 6 && hour <= 18) return 1;
    if (hour === 5 || hour === 19) return 0.8;
    if (hour === 4 || hour === 20) return 0.6;
    return 0.4;
};

const MapViewer = () => {
    const [tiles, setTiles] = useState([]);
    const [width, setWidth] = useState(0);
    const [height, setHeight] = useState(0);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const [selectedTile, setSelectedTile] = useState(null);
    const [viewportX, setViewportX] = useState(0);
    const [viewportY, setViewportY] = useState(0);
    const [tileSize, setTileSize] = useState(32);
    const [timeOfDay, setTimeOfDay] = useState(12);
    const [autoAdvance, setAutoAdvance] = useState(true);
    const [forward, setForward] = useState(true);

    const viewWidth = 15;
    const viewHeight = 10;
    const isNight = timeOfDay < 6 || timeOfDay > 18;

    const getWeatherSymbol = (weather) => weatherSymbols[weather] || "❔";
    const getOceanSymbol = (event) => oceanEventSymbols[event] || "⚠️";

    const renderGrid = () => {
        const grid = [];

        for (let y = viewportY; y < Math.min(viewportY + viewHeight, height); y++) {
            const row = [];
            for (let x = viewportX; x < Math.min(viewportX + viewWidth, width); x++) {
                const tile = tiles.find((t) => t?.x === x && t?.y === y);
                const content = tile ? (tile.oceanEvent ? getOceanSymbol(tile.oceanEvent) : getWeatherSymbol(tile.weather)) : "";
                const baseTerrain = tile ? formatTerrain(tile.terrain) : "Unknown";
                const effectiveTerrain = tile ? applyWeatherEffect(baseTerrain, tile.weather) : "Unknown";
                const localHour = tile ? getLocalHour(timeOfDay, tile.x, width) : 12;
                const brightness = getTileBrightness(localHour);
                const tileClass = `tile ${effectiveTerrain} ${tile?.weather} ${tile?.oceanEvent ? "ocean" : ""}`;
                const overlayIcon = terrainIcons[baseTerrain.toLowerCase()] || "";

                row.push(
                    <div
                        key={`${x}-${y}`}
                        className={tileClass}
                        style={{
                            width: tileSize,
                            height: tileSize,
                            fontSize: tileSize * 0.75,
                            filter: `brightness(${brightness})`,
                            position: "relative"
                        }}
                        onClick={() => tile && setSelectedTile(tile)}
                    >
                        {content}
                        {overlayIcon && <span className="overlay-icon">{overlayIcon}</span>}
                    </div>
                );
            }
            grid.push(<div key={y} className="row">{row}</div>);
        }

        return grid;
    };

    useEffect(() => {
        const interval = setInterval(() => {
            if (autoAdvance) {
                setTimeOfDay((prev) => forward ? (prev + 1) % 24 : (prev - 1 + 24) % 24);
            }
        }, 5000);
        return () => clearInterval(interval);
    }, [autoAdvance, forward]);

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
                setViewportX((prev) => Math.min(prev, maxX + 1 - viewWidth));
                setViewportY((prev) => Math.min(prev, maxY + 1 - viewHeight));
                setError(null);
            })
            .catch((err) => {
                console.error("Snapshot hatası:", err);
                setError(err.message);
            })
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        fetch("https://localhost:7260/api/map/generate")
            .then((res) => {
                if (!res.ok) throw new Error(`Harita oluşturulamadı: ${res.status}`);
                return res.json();
            })
            .then(() => {
                loadSnapshot();
                const interval = setInterval(() => loadSnapshot(), 5000);
                return () => clearInterval(interval);
            })
            .catch((err) => {
                console.error("İlk yükleme hatası:", err);
                setError(err.message);
            });
    }, []);

    useEffect(() => {
        const handleKeyDown = (e) => {
            if (e.key === "w") setViewportY((y) => Math.max(0, y - 1));
            if (e.key === "s") setViewportY((y) => Math.min(height - viewHeight, y + 1));
            if (e.key === "a") setViewportX((x) => Math.max(0, x - 1));
            if (e.key === "d") setViewportX((x) => Math.min(width - viewWidth, x + 1));
            if (e.key === "+") setTileSize((size) => Math.min(size + 4, 64));
            if (e.key === "-") setTileSize((size) => Math.max(size - 4, 16));
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [width, height]);

    return (
        <div className={`map ${getTimePhase(timeOfDay)}`}>
            {loading && <p>Yükleniyor...</p>}
            {error && <p style={{ color: "red" }}>Hata: {error}</p>}
            {!loading && !error && tiles.length > 0 && renderGrid()}

            <div style={{ marginTop: "10px" }}>
                <label htmlFor="timeSlider"><strong>Saat:</strong> {timeOfDay}:00</label><br />
                <input
                    id="timeSlider"
                    type="range"
                    min="0"
                    max="23"
                    value={timeOfDay}
                    onChange={(e) => setTimeOfDay(parseInt(e.target.value))}
                />
                <button onClick={() => setAutoAdvance(!autoAdvance)}>
                    {autoAdvance ? "Durdur" : "Zamanı Başlat"}
                </button>
                <button onClick={() => setForward(!forward)}>
                    {forward ? "⏩" : "⏪"}
                </button>
            </div>

            {selectedTile && (
                <div style={{ marginTop: "15px", padding: "8px", border: "1px solid #ccc", borderRadius: "5px", backgroundColor: "#fafafa" }}>
                    <strong>Koordinat:</strong> ({selectedTile.x}, {selectedTile.y})<br />
                    <strong>Terrain:</strong> {selectedTile.terrain}<br />
                    <strong>Weather:</strong> {selectedTile.weather}<br />
                    <strong>Global Time::</strong> {timeOfDay}:00 ({isNight ? "Night" : "Day"})<br />
                    <strong>Local Time::</strong> {getLocalHour(timeOfDay, selectedTile.x, width)}:00<br />
                    {selectedTile.oceanEvent && (
                        <><strong>Ocean Event:</strong> {selectedTile.oceanEvent}</>
                    )}
                </div>
            )}

            <div className="minimap" style={{ gridTemplateColumns: `repeat(${width}, 4px)` }}>
                {tiles.map((tile, index) => {
                    const isInViewport = tile.x >= viewportX && tile.x < viewportX + viewWidth && tile.y >= viewportY && tile.y < viewportY + viewHeight;
                    return (
                        <div
                            key={index}
                            className={`minitile ${isInViewport ? "viewport-tile" : ""}`}
                            title={`(${tile.x}, ${tile.y}) ${tile.terrain}`}
                            onClick={() => {
                                setViewportX(Math.max(0, Math.min(tile.x - Math.floor(viewWidth / 2), width - viewWidth)));
                                setViewportY(Math.max(0, Math.min(tile.y - Math.floor(viewHeight / 2), height - viewHeight)));
                            }}
                        />
                    );
                })}
            </div>
        </div>
    );
};

export default MapViewer;
