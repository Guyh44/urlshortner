using System;
using System.Data.SQLite;


namespace urlshortner.Services
{
    public class AddDB
    {
        private readonly string DBFile;
        public AddDB(string dbFile = "C:\\Users\\Hamburg\\source\\repos\\urlshortner\\UrlDataBase.db") // db path
        {
            DBFile = dbFile;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // if db doesnt exist create it
            if (!System.IO.File.Exists(DBFile))
            {
                //create db
                SQLiteConnection.CreateFile(DBFile);
            }
            
            using (var connection = new SQLiteConnection($"Data Source={DBFile};Version=3;")) //establish connection
            {
                connection.Open();
                // those are the filds of the table
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS URLS (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        OriginalUrl TEXT UNIQUE NOT NULL,
                        ShortCode TEXT UNIQUE NOT NULL,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void InsertValues(string longUrl, string shortUrl)
        {
            using (var connection = new SQLiteConnection($"Data Source={DBFile};Version=3;")) //connecting to db
            {
                connection.Open();

                string insertQuery = "INSERT INTO URLS (OriginalUrl, ShortCode) VALUES (@OriginalUrl, @ShortCode)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@OriginalUrl", longUrl);
                    command.Parameters.AddWithValue("@ShortCode", shortUrl);
                    command.ExecuteNonQuery();

                }
            }
        }

        public string GetLongUrlByShortCode(string shortcode)
        {
            using (var connection = new SQLiteConnection($"Data Source={DBFile};Version=3;"))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT OriginalUrl FROM URLS WHERE ShortCode = @ShortCode", connection))
                {
                    command.Parameters.AddWithValue("@ShortCode", shortcode);
                    var originalurl = command.ExecuteScalar();
                    return originalurl?.ToString() ?? string.Empty;
                }
            }
        }

        public string GetShortCodeByLongUrl(string longUrl)
        {
            using (var connection = new SQLiteConnection($"Data Source={DBFile};Version=3;"))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT ShortCode FROM URLS WHERE OriginalUrl = @OriginalUrl", connection))
                {
                    command.Parameters.AddWithValue("@OriginalUrl", longUrl);
                    var shortcode = command.ExecuteScalar();
                    return shortcode?.ToString() ?? string.Empty;
                }
            }
        }
    }
}
