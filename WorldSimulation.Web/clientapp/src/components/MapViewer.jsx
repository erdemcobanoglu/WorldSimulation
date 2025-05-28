import React, { useEffect, useState } from "react";
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

const wrapX = (x, width) => ((x % width) + width) % width;

const MapViewer = () => {
    const [tiles, setTiles] = useState([]);
    const [width, setWidth] = useState(0);
    const [height, setHeight] = useState(0);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const [selectedTile, setSelectedTile] = useState(null);
    const [viewportX, setViewportX] = useState(0);
    const [viewportY, setViewportY] = useState(0);
    const [tileSize, setTileSize] = useState(45);
    const [timeOfDay, setTimeOfDay] = useState(12);
    const [autoAdvance, setAutoAdvance] = useState(true);
    const [forward, setForward] = useState(true);
    const [startTile, setStartTile] = useState(null);
    const [endTile, setEndTile] = useState(null);
    const [path, setPath] = useState([]);

    const viewWidth = 15;
    const viewHeight = 10;
    const isNight = timeOfDay < 6 || timeOfDay > 18;

    const getWeatherSymbol = (weather) => weatherSymbols[weather] || "❔";
    const getOceanSymbol = (event) => oceanEventSymbols[event] || "⚠️";

    const isWalkable = (terrain) => ["Land", "Mud", "Ice", "Desert"].includes(terrain);

    const findPath = (start, end) => {
        const openSet = [start];
        const cameFrom = {};
        const gScore = {};
        const fScore = {};
        const key = (x, y) => `${x},${y}`;

        gScore[key(start.x, start.y)] = 0;
        fScore[key(start.x, start.y)] = Math.abs(start.x - end.x) + Math.abs(start.y - end.y);

        while (openSet.length > 0) {
            openSet.sort((a, b) => (fScore[key(a.x, a.y)] || Infinity) - (fScore[key(b.x, b.y)] || Infinity));
            const current = openSet.shift();

            if (current.x === end.x && current.y === end.y) {
                const path = [];
                let currKey = key(end.x, end.y);
                while (cameFrom[currKey]) {
                    path.unshift(cameFrom[currKey]);
                    currKey = key(cameFrom[currKey].x, cameFrom[currKey].y);
                }
                return path;
            }

            const neighbors = [
                { x: current.x + 1, y: current.y },
                { x: current.x - 1, y: current.y },
                { x: current.x, y: current.y + 1 },
                { x: current.x, y: current.y - 1 }
            ];

            for (const neighbor of neighbors) {
                const wrappedX = wrapX(neighbor.x, width);
                if (neighbor.y < 0 || neighbor.y >= height) continue;
                const tile = tiles.find((t) => t.x === wrappedX && t.y === neighbor.y);
                if (!tile) continue;
                const terrain = formatTerrain(tile.terrain);
                if (!isWalkable(terrain)) continue;

                const tentativeG = (gScore[key(current.x, current.y)] || Infinity) + 1;
                const neighborKey = key(wrappedX, neighbor.y);

                if (tentativeG < (gScore[neighborKey] || Infinity)) {
                    cameFrom[neighborKey] = current;
                    gScore[neighborKey] = tentativeG;
                    fScore[neighborKey] = tentativeG + Math.abs(wrappedX - end.x) + Math.abs(neighbor.y - end.y);
                    if (!openSet.find(t => t.x === wrappedX && t.y === neighbor.y)) {
                        openSet.push({ x: wrappedX, y: neighbor.y });
                    }
                }
            }
        }

        return [];
    };

    const renderGrid = () => {
        const grid = [];

        for (let y = viewportY; y < viewportY + viewHeight; y++) {
            const row = [];
            for (let x = viewportX; x < viewportX + viewWidth; x++) {
                const wrappedX = wrapX(x, width);
                const tile = tiles.find((t) => t?.x === wrappedX && t?.y === y);
                const content = tile ? (tile.oceanEvent ? getOceanSymbol(tile.oceanEvent) : getWeatherSymbol(tile.weather)) : "";
                const baseTerrain = tile ? formatTerrain(tile.terrain) : "Unknown";
                const effectiveTerrain = tile ? applyWeatherEffect(baseTerrain, tile.weather) : "Unknown";
                const localHour = tile ? getLocalHour(timeOfDay, tile.x, width) : 12;
                const brightness = getTileBrightness(localHour);
                const tileClass = `tile ${effectiveTerrain} ${tile?.weather} ${tile?.oceanEvent ? "ocean" : ""} ${path.some(p => p.x === wrappedX && p.y === y) ? "path" : ""}`;
                const overlayIcon = terrainIcons[baseTerrain.toLowerCase()] || "";

                row.push(
                    <div
                        key={`${wrappedX}-${y}`}
                        className={tileClass}
                        style={{
                            width: tileSize,
                            height: tileSize,
                            fontSize: tileSize * 0.75,
                            filter: `brightness(${brightness})`,
                            position: "relative"
                        }}
                        onClick={() => {
                            const clickedTile = tile;
                            setSelectedTile(clickedTile);

                            if (!startTile) setStartTile({ x: wrappedX, y });
                            else if (!endTile) {
                                setEndTile({ x: wrappedX, y });
                                const foundPath = findPath(startTile, { x: wrappedX, y });
                                setPath(foundPath);
                            } else {
                                setStartTile({ x: wrappedX, y });
                                setEndTile(null);
                                setPath([]);
                            }
                        }}
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
            if (e.key === "+") setTileSize((size) => Math.min(size + 4, 80));
            if (e.key === "-") setTileSize((size) => Math.max(size - 4, 24)); 
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [width, height]);

    return (
        <div className={`map ${getTimePhase(timeOfDay)}`} style={{
            display: "flex",
            flexDirection: "row",
            alignItems: "flex-start",
            justifyContent: "center",
            flexWrap: "wrap",
            gap: "20px",
            padding: "20px"
        }}>
            <div style={{ flex: "1 1 600px" }}>
                {loading && <p>Yükleniyor...</p>}
                {error && <p style={{ color: "red" }}>Hata: {error}</p>}
                {!loading && !error && tiles.length > 0 && renderGrid()}

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

            <div style={{
                padding: "10px",
                backgroundColor: "#1e1e2f",
                borderRadius: "10px",
                boxShadow: "0 0 10px #00000033",
                color: "#ffffff",
                minWidth: "280px",
                maxWidth: "360px",
                flex: "1 1 300px",
                position: "sticky",
                top: "20px"
            }}>
                <label htmlFor="timeSlider" style={{ fontWeight: "bold", fontSize: "1.1em" }}>🕒 Saat: <span style={{ fontFamily: "monospace" }}>{timeOfDay}:00</span></label>
                <input
                    id="timeSlider"
                    type="range"
                    min="0"
                    max="23"
                    value={timeOfDay}
                    onChange={(e) => setTimeOfDay(parseInt(e.target.value))}
                    style={{ width: "100%", marginTop: "8px", marginBottom: "12px" }}
                />

                <div style={{ display: "flex", justifyContent: "space-between", gap: "8px", marginBottom: "15px" }}>
                    <button onClick={() => setAutoAdvance(!autoAdvance)} style={{ padding: "6px 12px" }}>
                        {autoAdvance ? "⏸️ Durdur" : "▶️ Başlat"}
                    </button>
                    <button onClick={() => setForward(!forward)} style={{ padding: "6px 12px" }}>
                        {forward ? "⏩ İleri" : "⏪ Geri"}
                    </button>
                </div>

                {selectedTile && (
                    <div style={{
                        backgroundColor: "#2e2e3e",
                        padding: "12px",
                        borderRadius: "8px",
                        border: "1px solid #444",
                        fontSize: "0.95em",
                        lineHeight: "1.5em"
                    }}>
                        <div><strong>📍 Koordinat:</strong> ({selectedTile.x}, {selectedTile.y})</div>
                        <div><strong>🗺 Terrain:</strong> {selectedTile.terrain}</div>
                        <div><strong>🌦 Weather:</strong> {selectedTile.weather}</div>
                        <div><strong>🕓 Global:</strong> {timeOfDay}:00 <span style={{ color: isNight ? "#87cefa" : "#ffd700" }}>({isNight ? "Night" : "Day"})</span></div>
                        <div><strong>📍 Local:</strong> {getLocalHour(timeOfDay, selectedTile.x, width)}:00</div>
                        {selectedTile.oceanEvent && (
                            <div><strong>🌊 Ocean:</strong> {selectedTile.oceanEvent}</div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default MapViewer;
