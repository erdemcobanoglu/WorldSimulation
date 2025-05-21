import React, { useEffect, useState } from "react";
import "./WorldMapSimulation.css"; // Stil dosyası

function WorldMapSimulation() {
    const [mapData, setMapData] = useState(null);

    useEffect(() => {
        fetch("/api/map/generate")
            .then((res) => res.json())
            .then((data) => setMapData(data))
            .catch((err) => console.error("Hata:", err));
    }, []);

    if (!mapData) return <div>Harita yükleniyor...</div>;

    return (
        <div>
            <h2>Dünya Simülasyonu 🌍</h2>
            <div
                className="map-grid"
                style={{
                    gridTemplateColumns: `repeat(${mapData.width}, 20px)`,
                }}
            >
                {mapData.tiles.flat().map((tile, i) => (
                    <div
                        key={i}
                        className={`tile ${tile.type}`}
                        title={`(${tile.x}, ${tile.y}) - ${tile.type}`}
                    ></div>
                ))}
            </div>
        </div>
    );
}

export default WorldMapSimulation;
