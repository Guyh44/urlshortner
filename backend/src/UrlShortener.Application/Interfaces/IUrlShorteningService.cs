// this interface handles shortening URLs,
// retrieving original URLs, tracking click counts, and supporting custom codes

using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Interfaces
{
    public interface IUrlShorteningService
    {
        // shortens a given URL and optionally adding ttl
        // returns a response with the generated short URL
        Task<UrlShortenResponse> ShortenUrlAsync(UrlShortenRequest request);

        // shortens a given URL using an inputted custom short code
        Task<UrlShortenResponse> ShortenUrlWithCustomCodeAsync(CustomUrlShortenRequest request);

        // retrieves the original URL based on the provided short code
        // if the short code is expired or not found, returns null
        Task<string?> GetOriginalUrlAsync(string shortCode);

        // Returns the number of times the short URL has been accessed.
        // If the code doesn't exist, returns null.
        Task<int?> GetClickCountAsync(string shortCode);
    }
}