
// this service is responsible for handling all URL shortening logic,
// responsible for generation of short codes, handling custom codes, chacking that the url is valid,
// retrieving original URLs, and tracking click counts.

using UrlShortener.Application.DTOs;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Services;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICodeGenerator _codeGenerator;
    private const string BaseUrl = "http://localhost:5031/"; // the start of each url
    private const int ShortCodeLength = 6; // the length of the random short code

    public UrlShorteningService(IUrlRepository urlRepository, ICodeGenerator codeGenerator)
    {
        _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
    }
    
    
    // this func shortens a given URL using a randomly generated short code.
    // if the original URL already exists and is still valid, the func will return it
    public async Task<UrlShortenResponse> ShortenUrlAsync(UrlShortenRequest request)
    {
        var originalUrl = request.Url.Trim().ToLowerInvariant(); // gets the url from the user request

        if (!IsValidUrl(originalUrl)) // chacks if the url is a valid url
            throw new ArgumentException("Invalid URL format", nameof(request.Url));

        var existingUrl = await _urlRepository.GetByOriginalUrlAsync(originalUrl);
        if (existingUrl != null) // chacks if the url already is in the db
        {
            if (existingUrl.IsValid) // url haven't expired 
            {
                return new UrlShortenResponse(BaseUrl + existingUrl.ShortCode); // returns the existing url shortcode
            }
            else
            {
                // delete expired record before creating a new one because the existing url is expired
                await _urlRepository.DeleteAsync(existingUrl.ShortCode);
            }
        }

        string shortCode;
        do
        {
            shortCode = _codeGenerator.GenerateCode(ShortCodeLength);
        } while (await _urlRepository.ExistsAsync(shortCode)); // chacks that the newly created shortcode isn't taken

        var shortenedUrl = new ShortenedUrl(originalUrl, shortCode, request.Ttl); // create a new domain entity
        await _urlRepository.CreateAsync(shortenedUrl); // add to the db

        return new UrlShortenResponse(BaseUrl + shortCode); // return the new short url
    }
    
    // this func operates the same as "ShortenUrlAsync"
    // it shortens a given URL but with using a user input short code.
    // if the original URL already exists and is still valid, the func will return it
    public async Task<UrlShortenResponse> ShortenUrlWithCustomCodeAsync(CustomUrlShortenRequest request)
    {
        var originalUrl = request.Url.Trim().ToLowerInvariant(); // gets the url from the user request

        if (!IsValidUrl(originalUrl)) // chacks if the url is a valid url
            throw new ArgumentException("Invalid URL format", nameof(request.Url));

        if (string.IsNullOrWhiteSpace(request.CustomCode))  // chacks that the user shortcode is valid
            throw new ArgumentException("Custom code cannot be empty", nameof(request.CustomCode));
        
        // trying to fetch an exiting shortcode, that's like the newly inputted one
        var existingByCode = await _urlRepository.GetByShortCodeAsync(request.CustomCode);  
        if (existingByCode != null) // there is a shortcode like the new one in the db
        {
            if (existingByCode.IsValid) // chack that the one that exists is valid
            {
                throw new InvalidOperationException("Custom short code already in use.");
            }
            else
            {
                // delete expired record before creating a new one because the existing url is expired
                await _urlRepository.DeleteAsync(request.CustomCode);
            }
        }
        
        // now we are chacking if the url exist and not the short code
        var existingUrl = await _urlRepository.GetByOriginalUrlAsync(originalUrl);
        if (existingUrl != null) // chacks if the url already is in the db
        {
            if (existingUrl.IsValid) // url haven't expired 
            {
                return new UrlShortenResponse(BaseUrl + existingUrl.ShortCode); // returns the existing url shortcode
            }
            else
            {
                // delete expired record before creating a new one because the existing url is expired
                await _urlRepository.DeleteAsync(existingUrl.ShortCode);
            }
        }
        
        var shortenedUrl = new ShortenedUrl(originalUrl, request.CustomCode, request.Ttl); // create a new domain entity
        await _urlRepository.CreateAsync(shortenedUrl); // adding that entity to the db

        return new UrlShortenResponse(BaseUrl + request.CustomCode); // // return the new custom short url
    }

    
    // retrieves the original URL for the given short code
    // if the URL is expired or not found, the func will return null
    public async Task<string?> GetOriginalUrlAsync(string shortCode)
    {
        var shortenedUrl = await _urlRepository.GetByShortCodeAsync(shortCode);
        if (shortenedUrl == null || shortenedUrl.IsExpired)
        {
            if (shortenedUrl != null)
                await _urlRepository.DeleteAsync(shortCode);
            return null;
        }

        shortenedUrl.RegisterClick();
        await _urlRepository.UpdateAsync(shortenedUrl);

        return shortenedUrl.OriginalUrl;
    }

    
    // gets the number of times a short url was used from the db
    public async Task<int?> GetClickCountAsync(string shortCode)
    {
        var shortened = await _urlRepository.GetByShortCodeAsync(shortCode);
        return shortened?.ClickCount;
    }

    // chacks that the url is well formatted (https://....)
    private static bool IsValidUrl(string url) =>
        Uri.IsWellFormedUriString(url, UriKind.Absolute);
}
