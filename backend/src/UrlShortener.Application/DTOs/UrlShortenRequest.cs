namespace UrlShortener.Application.DTOs;

public record UrlShortenRequest(string Url, int TTL = 0);

public record CustomUrlShortenRequest(string Url, string CustomCode, int TTL = 0);

public record UrlShortenResponse(string ShortUrl);