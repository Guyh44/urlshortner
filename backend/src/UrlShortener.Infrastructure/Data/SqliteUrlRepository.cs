using System.Data.Common;
using System.Data.SQLite;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Data
{
    public class SqliteUrlRepository : IUrlRepository
    {
        private readonly string _connectionString;

        public SqliteUrlRepository(string? dbFilePath = null)
        {
            var dbFile = dbFilePath ?? GetDefaultDbPath();
            _connectionString = $"Data Source={dbFile};Version=3;";
            InitializeDatabase();
        }

        private static string GetDefaultDbPath()
        {
            var projectRoot = Directory.GetCurrentDirectory();
            
            var dataFolder = Path.Combine(projectRoot, "Data");
            Directory.CreateDirectory(dataFolder);
            
            return Path.Combine(dataFolder, "UrlDatabase.db");
        }

        private void InitializeDatabase()
        {
            var dbFile = ExtractDbFileFromConnectionString();
            if (!File.Exists(dbFile))
                SQLiteConnection.CreateFile(dbFile);

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS URLS (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OriginalUrl TEXT UNIQUE COLLATE NOCASE NOT NULL,
                    ShortCode TEXT UNIQUE NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    ExpiresAt DATETIME NULL,
                    ClickCount INT NOT NULL
                )";

            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        private string ExtractDbFileFromConnectionString()
        {
            var parts = _connectionString.Split(';');
            var dataSourcePart = parts
                .FirstOrDefault(p => p.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Invalid connection string");

            return dataSourcePart.Substring("Data Source=".Length);
        }

        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt, ClickCount
                  FROM URLS
                 WHERE ShortCode = @ShortCode";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ShortCode", shortCode);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return CreateShortenedUrlFromReader(reader);
        }

        public async Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt, ClickCount
                  FROM URLS
                 WHERE OriginalUrl = @OriginalUrl COLLATE NOCASE";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@OriginalUrl", originalUrl.Trim().ToLowerInvariant());

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return CreateShortenedUrlFromReader(reader);
        }

        public async Task<ShortenedUrl> CreateAsync(ShortenedUrl shortenedUrl)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string insertQuery = @"
                INSERT INTO URLS
                    (OriginalUrl, ShortCode, CreatedAt, ExpiresAt, ClickCount)
                VALUES
                    (@OriginalUrl, @ShortCode, @CreatedAt, @ExpiresAt, @ClickCount)";

            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@OriginalUrl", shortenedUrl.OriginalUrl);
            command.Parameters.AddWithValue("@ShortCode", shortenedUrl.ShortCode);
            command.Parameters.AddWithValue("@CreatedAt", shortenedUrl.CreatedAt);
            command.Parameters.AddWithValue("@ExpiresAt", (object?)shortenedUrl.ExpiresAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@ClickCount", shortenedUrl.ClickCount);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            {
                throw new InvalidOperationException(
                    "A shortened URL with this OriginalUrl or ShortCode already exists.");
            }

            return shortenedUrl;
        }

        public async Task UpdateAsync(ShortenedUrl shortenedUrl)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string updateQuery = @"
                UPDATE URLS
                   SET ClickCount = @ClickCount
                 WHERE ShortCode = @ShortCode";

            using var command = new SQLiteCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@ClickCount", shortenedUrl.ClickCount);
            command.Parameters.AddWithValue("@ShortCode", shortenedUrl.ShortCode);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(string shortCode)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SQLiteCommand(
                "DELETE FROM URLS WHERE ShortCode = @ShortCode",
                connection);

            command.Parameters.AddWithValue("@ShortCode", shortCode);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(string shortCode)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SQLiteCommand(
                "SELECT COUNT(1) FROM URLS WHERE ShortCode = @ShortCode",
                connection);

            command.Parameters.AddWithValue("@ShortCode", shortCode);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        private static ShortenedUrl CreateShortenedUrlFromReader(DbDataReader reader)
        {
            // Use the private ctor to hydrate the entity
            var shortenedUrl = (ShortenedUrl)Activator
                .CreateInstance(typeof(ShortenedUrl), nonPublic: true)!;

            // Map all columns, including ClickCount
            typeof(ShortenedUrl).GetProperty("Id")!
                .SetValue(shortenedUrl, Convert.ToInt32(reader["Id"]));
            typeof(ShortenedUrl).GetProperty("OriginalUrl")!
                .SetValue(shortenedUrl, reader["OriginalUrl"].ToString());
            typeof(ShortenedUrl).GetProperty("ShortCode")!
                .SetValue(shortenedUrl, reader["ShortCode"].ToString());
            typeof(ShortenedUrl).GetProperty("CreatedAt")!
                .SetValue(shortenedUrl, Convert.ToDateTime(reader["CreatedAt"]));
            typeof(ShortenedUrl).GetProperty("ExpiresAt")!
                .SetValue(shortenedUrl,
                    reader["ExpiresAt"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["ExpiresAt"]));
            typeof(ShortenedUrl).GetProperty("ClickCount")!
                .SetValue(shortenedUrl, Convert.ToInt32(reader["ClickCount"]));

            return shortenedUrl;
        }
    }
}
