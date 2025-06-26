using System.Data.Common;
using System.Data.SQLite;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Data;

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
        var folder = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "UrlDatabase.db");
    }


    private void InitializeDatabase()
    {
        var dbFile = ExtractDbFileFromConnectionString();
        if (!File.Exists(dbFile))
        {
            SQLiteConnection.CreateFile(dbFile);
        }

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS URLS (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OriginalUrl TEXT UNIQUE COLLATE NOCASE NOT NULL,
                ShortCode TEXT UNIQUE NOT NULL,
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NULL
            )";

        using var command = new SQLiteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    private string ExtractDbFileFromConnectionString()
    {
        var parts = _connectionString.Split(';');
        var dataSourcePart = parts.FirstOrDefault(p => p.Trim().StartsWith("Data Source="));
        return dataSourcePart?.Substring("Data Source=".Length) ?? throw new InvalidOperationException("Invalid connection string");
    }

    public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
    {
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt FROM URLS WHERE ShortCode = @ShortCode";
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

        const string query = "SELECT Id, OriginalUrl, ShortCode, CreatedAt, ExpiresAt FROM URLS WHERE OriginalUrl = @OriginalUrl COLLATE NOCASE";
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
            INSERT INTO URLS (OriginalUrl, ShortCode, CreatedAt, ExpiresAt) 
            VALUES (@OriginalUrl, @ShortCode, @CreatedAt, @ExpiresAt)";

        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@OriginalUrl", shortenedUrl.OriginalUrl);
        command.Parameters.AddWithValue("@ShortCode", shortenedUrl.ShortCode);
        command.Parameters.AddWithValue("@CreatedAt", shortenedUrl.CreatedAt);
        command.Parameters.AddWithValue("@ExpiresAt", (object?)shortenedUrl.ExpiresAt ?? DBNull.Value);

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
        {
            throw new InvalidOperationException("A shortened URL with this OriginalUrl or ShortCode already exists.");
        }

        return shortenedUrl;
    }

    public async Task DeleteAsync(string shortCode)
    {
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand("DELETE FROM URLS WHERE ShortCode = @ShortCode", connection);
        command.Parameters.AddWithValue("@ShortCode", shortCode);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> ExistsAsync(string shortCode)
    {
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand("SELECT COUNT(1) FROM URLS WHERE ShortCode = @ShortCode", connection);
        command.Parameters.AddWithValue("@ShortCode", shortCode);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    private static ShortenedUrl CreateShortenedUrlFromReader(DbDataReader reader)
    {
        var shortenedUrl = (ShortenedUrl)Activator.CreateInstance(typeof(ShortenedUrl), true)!;

        typeof(ShortenedUrl).GetProperty("Id")!.SetValue(shortenedUrl, Convert.ToInt32(reader["Id"]));
        typeof(ShortenedUrl).GetProperty("OriginalUrl")!.SetValue(shortenedUrl, reader["OriginalUrl"].ToString());
        typeof(ShortenedUrl).GetProperty("ShortCode")!.SetValue(shortenedUrl, reader["ShortCode"].ToString());
        typeof(ShortenedUrl).GetProperty("CreatedAt")!.SetValue(shortenedUrl, Convert.ToDateTime(reader["CreatedAt"]));
        typeof(ShortenedUrl).GetProperty("ExpiresAt")!.SetValue(shortenedUrl,
            reader["ExpiresAt"] == DBNull.Value ? null : Convert.ToDateTime(reader["ExpiresAt"]));

        return shortenedUrl;
    }
}
