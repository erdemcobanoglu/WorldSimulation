import React from "react";
import MapViewer from "./components/MapViewer"; // dosya yolu doğruysa

function App() {
    return (
        <div className="App">
            <h2>Hava Durumu Haritası</h2>
            <MapViewer />
        </div>
    );
}

export default App;
