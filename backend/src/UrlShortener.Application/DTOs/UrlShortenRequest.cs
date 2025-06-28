namespace UrlShortener.Application.DTOs
{
    // this model represents a normal request without a custom short code
    public record UrlShortenRequest(string Url, int Ttl = 0);

    // this model represents a request with a custom short code
    public record CustomUrlShortenRequest(string Url, string CustomCode, int Ttl = 0);

    // this model represents the response sent back to the user after a successful shortening request
    public record UrlShortenResponse(string ShortUrl);
}