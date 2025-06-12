// src/api/urlShortenerApi.js

export async function shortenWithRandomCode(url) {
  const response = await fetch("http://localhost:5235/api/shorten", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ url }),
  });

  return response;
}

export async function shortenWithCustomCode(url, customCode) {
  const response = await fetch("http://localhost:5235/api/custom/shorten", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ url, customCode }),
  });

  return response;
}
