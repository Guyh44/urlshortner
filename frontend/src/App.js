import React from "react";
import "./App.css";
import handleSubmit from "./buttonHandler.js";

function App() {
  return (
    <div className="app-container">
      <main>
        <h1>Ever dreamed of shortening a URL?</h1>
        <div className="box">
          <h2 className="sub-headers">Please enter a URL:</h2>
          <input
            type="text"
            placeholder="https://example.com"
            className="input"
          />
          <h2 className="sub-headers">Please enter a custom short code:</h2>
          <input
            type="text"
            placeholder="custom short code"
            className="input-short-code"
          />
          <button
            type="submit"
            className="submit-btn"
            onClick={handleSubmit}
          >
            Convert URL
          </button>

        </div>
      </main>

      <hr className="footer-divider" />
      <footer>© All rights reserved to Guy Harpaz</footer>
    </div>
  );
}

export default App;
