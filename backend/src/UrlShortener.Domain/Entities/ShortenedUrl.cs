namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public int Id { get; private set; }
    public string OriginalUrl { get; private set; }
    public string ShortCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public ShortenedUrl(string originalUrl, string shortCode, int ttlMinutes = 0)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            throw new ArgumentException("Original URL cannot be null or empty", nameof(originalUrl));
        
        if (string.IsNullOrWhiteSpace(shortCode))
            throw new ArgumentException("Short code cannot be null or empty", nameof(shortCode));

        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = ttlMinutes > 0 ? CreatedAt.AddMinutes(ttlMinutes) : null;
    }

    // For EF Core
    private ShortenedUrl()
    {
        OriginalUrl = string.Empty; //must have value (cannot bee null will be overwritten)
        ShortCode = string.Empty; //must have value (cannot bee null will be overwritten)
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    public bool IsValid => !IsExpired;
}