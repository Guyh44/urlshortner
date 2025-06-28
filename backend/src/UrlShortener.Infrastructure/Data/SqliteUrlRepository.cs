// this class implements IUrlRepository using SQLite as the underlying storage engine.
// it handles all interactions with the database, including creation, retrieval, updates,
// deletions, and existence checks for shortened URLs

using System.Data.Common;
using System.Data.SQLite;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Data
{
    public class SqliteUrlRepository : IUrlRepository
    {
        private readonly string _connectionString;
        
        // strats the repository with a connection string and creates the db if it doesn't exist
        public SqliteUrlRepository(string? dbFilePath = null)
        {
            var dbFile = dbFilePath ?? GetDefaultDbPath();
            _connectionString = $"Data Source={dbFile};Version=3;DateTimeKind=Utc;";
            InitializeDatabase(); 
        }
        // determines where the database file should be created,
        // will always be in backend/src/UrlShortener.API/Data/UrlDatabase.db
        private static string GetDefaultDbPath()
        {
            var projectRoot = Directory.GetCurrentDirectory();
            
            var dataFolder = Path.Combine(projectRoot, "Data");
            Directory.CreateDirectory(dataFolder); // ensures the folder exists
            return Path.Combine(dataFolder, "UrlDatabase.db");
        }

        // creates the database file and URLS table if they don't exist
        private void InitializeDatabase()
        {
            var dbFile = ExtractDbFileFromConnectionString();
            if (!File.Exists(dbFile)) // chacks if the db file already exists
                SQLiteConnection.CreateFile(dbFile);

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            
            // create the table and its properties 
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
        
        // returns only the file path from the connection string
        private string ExtractDbFileFromConnectionString()
        {
            var parts = _connectionString.Split(';');
            var dataSourcePart = parts
                .FirstOrDefault(p => p.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Invalid connection string");

            return dataSourcePart.Substring("Data Source=".Length);
        }
        
        // retrieves a shortened URL entity by short code
        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
        {
            // connects to the db
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync(); 
            
            // gets all the properties by the shortcode
            const string query = @"
                SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt, ClickCount
                  FROM URLS
                 WHERE ShortCode = @ShortCode";
            
            // create a new SQLite command with the provided SQL query and open connection
            using var command = new SQLiteCommand(query, connection);
            
            // add the shortCode value as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@ShortCode", shortCode);

            using var reader = await command.ExecuteReaderAsync();
            
            // if the reader has no rows that mean the db didn't find anything, return null
            if (!await reader.ReadAsync()) return null;
            
            return CreateShortenedUrlFromReader(reader);
        }

        // retrieves a shortened URL entity by the original long URL
        public async Task<ShortenedUrl?> GetByOriginalUrlAsync(string originalUrl)
        {
            // connects to the db
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            
            // gets all the properties by the original URL
            const string query = @"
                SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt, ClickCount
                  FROM URLS
                 WHERE OriginalUrl = @OriginalUrl COLLATE NOCASE";

            // create a new SQLite command with the provided SQL query and open connection
            using var command = new SQLiteCommand(query, connection);
            
            // add the original URL value as a parameter to the query to prevent SQL injection
            command.Parameters.AddWithValue("@OriginalUrl", originalUrl.Trim().ToLowerInvariant());

            using var reader = await command.ExecuteReaderAsync();
            // if the reader has no rows that mean the db didn't find anything, return null
            if (!await reader.ReadAsync()) return null;

            return CreateShortenedUrlFromReader(reader);
        }
    
        // inserts a new shortened URL record into the database
        public async Task<ShortenedUrl> CreateAsync(ShortenedUrl shortenedUrl)
        {
            // connects to the db
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            
            // preparing to insert all the values from the new request into the query
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
                // handles duplicate OriginalUrl or ShortCode constraint violation
                throw new InvalidOperationException(
                    "A shortened URL with this OriginalUrl or ShortCode already exists.");
            }

            return shortenedUrl;
        }
        
        //update the number of times a short url was used in the db
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
        
        // deletes a url from the db by its shortcode
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
        
        // checks if a short code already exists in the URLS table
        public async Task<bool> ExistsAsync(string shortCode)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            // chacks how many time does the short code exist in the db, will always be 0 or 1
            // i do it like that because it's faster than retrieving the whole row data,
            // just counting the number of matches
            using var command = new SQLiteCommand(
                "SELECT COUNT(1) FROM URLS WHERE ShortCode = @ShortCode",
                connection);

            command.Parameters.AddWithValue("@ShortCode", shortCode);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // covert the result to int and chacks if its bigger then 0
        }

        // creates a ShortenedUrl object from a database reader row,
        // i want the whole code to work with ShortenedUrl entity and now with the db to follow clean architecture logic
        private static ShortenedUrl CreateShortenedUrlFromReader(DbDataReader reader)
        {
            // use the private ctor create a shortenedurl entity out of the db row
            var shortenedUrl = (ShortenedUrl)Activator
                .CreateInstance(typeof(ShortenedUrl), nonPublic: true)!;

            // map all columns from the db to the shortenurl entity
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
