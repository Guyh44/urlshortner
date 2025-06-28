// this code represents a shortened URL entity used in the application.
//
// each instance tracks the original long URL, a unique short code,
// creation time, optional expiration time, and total number of times the site was visited


namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    //represents the id of the instance
    public int Id { get; private set; }
    
    //represents the original URL that the user inputs
    public string OriginalUrl { get; private set; }
    
    //represents the random short-code that the code generates
    public string ShortCode { get; private set; }
    
    //represents the time the instance was created
    public DateTime CreatedAt { get; private set; }
    
    // represents expiration time in minutes and applied from the moment of creation,
    // all timestamps are stored in UTC to ensure consistent expiration logic even tho we are in israel
    public DateTime? ExpiresAt { get; private set; }
    
    //represents the amount of time the shorted url was used
    public int ClickCount { get; private set; } 
    
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
        ClickCount = 0;
    }
    
    public void RegisterClick()
    {
        ClickCount++;
    }

    // For EF Core - allows the code to create an empty object,
    // so when it read from the db it will input the values here
    private ShortenedUrl()
    {
        OriginalUrl = string.Empty; //must have value (cannot be null, will be overwritten)
        ShortCode = string.Empty; //must have value (cannot be null, will be overwritten)
    }

    // the IsExpired and IsValid properties provide easy checks for link validity
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    public bool IsValid => !IsExpired; // i made it so the code will be cleaner instead of !IsExpire everytime
}