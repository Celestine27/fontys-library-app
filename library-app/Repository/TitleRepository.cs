using library_app.Models;
using MySqlConnector;

namespace library_app.Repository;

public class TitleRepository
{
    private readonly string? _connectionString;

    public TitleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<Title>> GetAllTitlesAsync()
    {
        List<Title> titles = [];

        await using MySqlConnection connection = new MySqlConnection(_connectionString); // Create a connection object
        await connection.OpenAsync(); // Opens the connection to the database

        string query = "select id, name, creator, isbn_number, max_chapters from titles"; // Selects columns from the table

        await using MySqlCommand command = new MySqlCommand(query, connection); // Prepares the SQL command
        await using MySqlDataReader reader = await command.ExecuteReaderAsync(); // Executes the query and reads the rows
        
        while (await reader.ReadAsync())
        {
            string? isbn = null; // Sets default value to null in case the database field is empty
            
            // IsDBNull checks if the value is NULL before reading and GetOrdinal gets the column index
            if (!reader.IsDBNull(reader.GetOrdinal("isbn_number"))) 
            {
                isbn = reader.GetString("isbn_number");
            }

            int? maxChapters = null;

            if (!reader.IsDBNull(reader.GetOrdinal("max_chapters")))
            {
                maxChapters = reader.GetInt32("max_chapters");
            }
            
            Title title = new Title
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Creator = reader.GetString("creator"),
                IsbnNumber = isbn,
                MaxChapters = maxChapters
            };
            
            titles.Add(title);
        }

        return titles;
    }
}