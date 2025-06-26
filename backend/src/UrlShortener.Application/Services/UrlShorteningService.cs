using UrlShortener.Application.DTOs;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Services;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICodeGenerator _codeGenerator;
    private const string BaseUrl = "http://localhost:5031/";
    private const int ShortCodeLength = 6;

    public UrlShorteningService(IUrlRepository urlRepository, ICodeGenerator codeGenerator)
    {
        _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
    }

    public async Task<UrlShortenResponse> ShortenUrlAsync(UrlShortenRequest request)
    {
        var normalizedUrl = request.Url.Trim().ToLowerInvariant();

        if (!IsValidUrl(normalizedUrl))
            throw new ArgumentException("Invalid URL format", nameof(request.Url));

        var existingUrl = await _urlRepository.GetByOriginalUrlAsync(normalizedUrl);
        if (existingUrl != null)
        {
            if (existingUrl.IsValid)
            {
                return new UrlShortenResponse(BaseUrl + existingUrl.ShortCode);
            }
            else
            {
                // Delete expired record before creating a new one
                await _urlRepository.DeleteAsync(existingUrl.ShortCode);
            }
        }

        string shortCode;
        do
        {
            shortCode = _codeGenerator.GenerateCode(ShortCodeLength);
        } while (await _urlRepository.ExistsAsync(shortCode));

        var shortenedUrl = new ShortenedUrl(normalizedUrl, shortCode, request.TTL);
        await _urlRepository.CreateAsync(shortenedUrl);

        return new UrlShortenResponse(BaseUrl + shortCode);
    }

    public async Task<UrlShortenResponse> ShortenUrlWithCustomCodeAsync(CustomUrlShortenRequest request)
    {
        var normalizedUrl = request.Url.Trim().ToLowerInvariant();

        if (!IsValidUrl(normalizedUrl))
            throw new ArgumentException("Invalid URL format", nameof(request.Url));

        if (string.IsNullOrWhiteSpace(request.CustomCode))
            throw new ArgumentException("Custom code cannot be empty", nameof(request.CustomCode));

        var existingByCode = await _urlRepository.GetByShortCodeAsync(request.CustomCode);
        if (existingByCode != null)
        {
            if (existingByCode.IsValid)
            {
                throw new InvalidOperationException("Custom short code already in use.");
            }
            else
            {
                // Delete expired record with this custom code
                await _urlRepository.DeleteAsync(request.CustomCode);
            }
        }

        var existingUrl = await _urlRepository.GetByOriginalUrlAsync(normalizedUrl);
        if (existingUrl != null)
        {
            if (existingUrl.IsValid)
            {
                return new UrlShortenResponse(BaseUrl + existingUrl.ShortCode);
            }
            else
            {
                // Delete expired record before inserting new one
                await _urlRepository.DeleteAsync(existingUrl.ShortCode);
            }
        }

        var shortenedUrl = new ShortenedUrl(normalizedUrl, request.CustomCode, request.TTL);
        await _urlRepository.CreateAsync(shortenedUrl);

        return new UrlShortenResponse(BaseUrl + request.CustomCode);
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode)
    {
        var shortenedUrl = await _urlRepository.GetByShortCodeAsync(shortCode);

        if (shortenedUrl == null || !shortenedUrl.IsValid)
        {
            if (shortenedUrl != null)
            {
                await _urlRepository.DeleteAsync(shortCode);
            }
            return null;
        }
        shortenedUrl.RegisterClick();
        await _urlRepository.UpdateAsync(shortenedUrl);
        return shortenedUrl.OriginalUrl;
    }
    
    public async Task<int?> GetClickCountAsync(string shortCode)
    {
        var shortened = await _urlRepository.GetByShortCodeAsync(shortCode);
        return shortened?.ClickCount;
    }

    
    private static bool IsValidUrl(string url) =>
        Uri.IsWellFormedUriString(url, UriKind.Absolute);
}
