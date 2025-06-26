using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces
{
    public interface IUrlRepository
    {
        // Finds a shortened URL record by its shortcode.
        // Returns null if not found.
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
        
        // Finds a shortened URL record by its original URL.
        // Returns null if not found.
        Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl);
        
        // Persists a new shortened URL record.
        Task<ShortenedUrl> CreateAsync(ShortenedUrl shortenedUrl);
        
        // Deletes a shortened URL record by its shortcode.
        Task DeleteAsync(string shortCode);
        
        // Returns true if a record with the given shortcode exists.
        Task<bool> ExistsAsync(string shortCode);
        // save the updated clickCount
        Task UpdateAsync(ShortenedUrl shortenedUrl);
    }
}
