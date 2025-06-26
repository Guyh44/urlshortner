

export async function shortenWithRandomCode(url, ttl = 0) {
  const response = await fetch("http://localhost:5031/api/shorten", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ url, TTL: ttl }),
  });

  return response;
}

export async function shortenWithCustomCode(url, customCode, ttl = 0) {
  const response = await fetch("http://localhost:5235/api/custom/shorten", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ url, customCode, TTL: ttl }),
  });

  return response;
}
