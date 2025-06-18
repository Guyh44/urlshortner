import React, { useState } from "react";
import "./App.css";
import handleSubmit from "./buttonHandler";

function App() {
  const [url, setUrl] = useState("");
  const [customCode, setCustomCode] = useState("");
  const [ttl, setTtl] = useState(0);
  const [shortenedUrl, setShortenedUrl] = useState("");
  const [error, setError] = useState("");

  const onFormSubmit = async () => {
    setError(""); // clear previous error
    setShortenedUrl(""); // clear previous result

    try {
      const result = await handleSubmit(url, customCode);
      setShortenedUrl(result.shortUrl);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="app-container">
      <main>
        <h1>Ever dreamed about shortening a URL?</h1>
        <div className="box">
          <h2 className="sub-headers">Please enter a URL:</h2>
          <input
            type="text"
            placeholder="https://example.com"
            className="input"
            value={url}
            onChange={(e) => setUrl(e.target.value)}
          />
          <h2 className="sub-headers">Please enter a custom short code:</h2>
          <input
            type="text"
            placeholder="custom short code"
            className="input-short-code"
            value={customCode}
            onChange={(e) => setCustomCode(e.target.value)}
          />
          <h2 className="sub-headers">TTL in minutes (0 = permanent):</h2>
          <input
            type="number"
            placeholder="0"
            className="input"
            value={ttl}
            onChange={(e) => setTtl(parseInt(e.target.value) || 0)}
            min="0"
          />
          <button 
            type="submit" 
            className="submit-btn" 
            onClick={onFormSubmit}>
            Convert URL
          </button>

          {error && (
            <p className="error-message" style={{ color: "red", marginTop: "1rem" }}>
              {error}
            </p>
          )}

          <h2 className="sub-headers" style={{ marginTop: "1rem" }}>
            Shortened URL:
          </h2>
          <input
            type="text"
            className="input"
            value={shortenedUrl}
            readOnly
            placeholder="Result will appear here"
          />
        </div>
      </main>

      <hr className="footer-divider" />
      <footer>© All rights reserved to Guy Harpaz</footer>
    </div>
  );
}

export default App;
