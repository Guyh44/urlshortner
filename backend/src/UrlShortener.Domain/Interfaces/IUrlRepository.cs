using System.Threading.Tasks;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces
{
    public interface IUrlRepository
    {
        /// <summary>
        /// Finds a shortened URL record by its shortcode.
        /// Returns null if not found.
        /// </summary>
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);

        /// <summary>
        /// Finds a shortened URL record by its original URL.
        /// Returns null if not found.
        /// </summary>
        Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl);

        /// <summary>
        /// Persists a new shortened URL record.
        /// </summary>
        Task<ShortenedUrl> CreateAsync(ShortenedUrl shortenedUrl);

        /// <summary>
        /// Deletes a shortened URL record by its shortcode.
        /// </summary>
        Task DeleteAsync(string shortCode);

        /// <summary>
        /// Returns true if a record with the given shortcode exists.
        /// </summary>
        Task<bool> ExistsAsync(string shortCode);
    }
}
