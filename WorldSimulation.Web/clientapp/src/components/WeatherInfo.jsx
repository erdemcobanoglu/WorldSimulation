import React, { useEffect, useState } from "react";

const WeatherInfo = () => {
    const [data, setData] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch("https://localhost:7260/api/map/generatev2")
            .then((res) => {
                if (!res.ok) throw new Error("API hatası");
                return res.json();
            })
            .then((data) => setData(data))
            .catch((err) => {
                console.error("Fetch hatası:", err);
                setError(err.message);
            });
    }, []);

    if (error) return <div>Hata: {error}</div>;
    if (!data) return <div>Yükleniyor...</div>;

    return (
        <div>
            <h3>Simülasyon Sonucu</h3>
            <ul>
                <li>ID: {data.id}</li>
                <li>Şehir: {data.city}</li>
                <li>Hava: {data.weather}</li>
                <li>Değer: {data.value}°C</li>
                <li>Zaman: {new Date(data.timestamp).toLocaleString()}</li>
            </ul>
        </div>
    );
};

export default WeatherInfo;
