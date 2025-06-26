using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Interfaces;

public interface IUrlShorteningService
{
    Task<UrlShortenResponse> ShortenUrlAsync(UrlShortenRequest request);
    Task<UrlShortenResponse> ShortenUrlWithCustomCodeAsync(CustomUrlShortenRequest request);
    Task<string?> GetOriginalUrlAsync(string shortCode);
    Task<int?> GetClickCountAsync(string shortCode);

}