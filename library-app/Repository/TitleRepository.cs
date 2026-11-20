using library_app.Models;
using MySqlConnector;

namespace library_app.Repository;

public class TitleRepository : ITitleRepository
{
    private readonly string? _connectionString;

    public TitleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Creates the connection object and opens the connection
    private async Task<MySqlConnection> GetOpenConnectionAsync()
    { 
        MySqlConnection connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        return connection;
    }

    public async Task<List<Title>> GetAllTitlesAsync()
    {
        List<Title> titles = [];

        await using MySqlConnection connection = await GetOpenConnectionAsync();

        string query = """
                       select 
                       id, 
                       name, 
                       creator, 
                       isbn,
                       total_chapters
                       from titles
                       """; // Selects columns from the table

        await using MySqlCommand command = new MySqlCommand(query, connection); // Prepares the SQL command
        await using MySqlDataReader reader = await command.ExecuteReaderAsync(); // Executes the query and reads the rows
        
        while (await reader.ReadAsync())
        {
            string? isbn = null; // Sets default value to null in case the database field is empty
            
            // IsDBNull checks if the value is NULL before reading and GetOrdinal gets the column index
            if (!reader.IsDBNull(reader.GetOrdinal("isbn"))) 
            {
                isbn = reader.GetString("isbn");
            }

            int? totalChapters = null;

            if (!reader.IsDBNull(reader.GetOrdinal("total_chapters")))
            {
                totalChapters = reader.GetInt32("total_chapters");
            }
            
            Title title = new Title
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Creator = reader.GetString("creator"),
                Isbn = isbn,
                TotalChapters = totalChapters
            };
            
            titles.Add(title);
        }

        return titles;
    }

    public async Task<bool> AddTitleAsync(Title title)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();

        string query = """
                        insert into titles(name,creator, isbn, total_chapters)
                        values(@name, @creator, @isbn, @total_chapters)
                       """; // Inserts values in specified column

        await using MySqlCommand command = new MySqlCommand(query, connection); // Prepares the SQL command

        // Executes the SQL commands
        command.Parameters.AddWithValue("@name", title.Name);
        command.Parameters.AddWithValue("@creator", title.Creator);
        
        // Checks if the value is null
        command.Parameters.AddWithValue("@isbn", DBNull.Value);
        if (title.Isbn != null)
        {
            command.Parameters["@isbn"].Value = title.Isbn;
        }
        
        // Checks if the value is null
        command.Parameters.AddWithValue("@total_chapters", DBNull.Value);
        if (title.TotalChapters != null)
        {
            command.Parameters["@total_chapters"].Value = title.TotalChapters;
        }
        
        int rowsAffected = await command.ExecuteNonQueryAsync(); // Gives a number of affected rows
        
        return rowsAffected > 0;
    }
}