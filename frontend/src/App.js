import React from "react";
import "./App.css";
import hanldeSubmit from "./buttonHandler.js";


function App() {
  return (
    <div className="app-container">
      <div className="box">
        <h1>
          hello welcome to my url shortner porject
        </h1>
        <h2 className="enter-url">
          please enter an URL:
        </h2>
        <input
          type="text"
          placeholder="original URL..."
          className="url-input"
        />
        <button
        type="submit"
        className="submit-btn"
        onClick={hanldeSubmit}
        >
          <span>convert url</span>
        </button>
      </div>
    </div>
  );
}

export default App;
