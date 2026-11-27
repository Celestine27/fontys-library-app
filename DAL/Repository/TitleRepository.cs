using Logic.Interfaces;
using Logic.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DAL.Repository;

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
                       titles.id, 
                       name, 
                       creator, 
                       isbn,
                       total_chapters,
                       type_id,
                       total_pages,
                       total_volumes
                       from titles
                       left join books on titles.id = books.id
                       left join manga on titles.id = manga.id
                       """; // Selects columns from the table

        await using MySqlCommand command = new MySqlCommand(query, connection); // Prepares the SQL command
        await using MySqlDataReader reader = await command.ExecuteReaderAsync(); // Executes the query and reads the rows
        
        while (await reader.ReadAsync())
        {
            Title title = MapReaderToTitle(reader);
            titles.Add(title);
        }

        return titles;
    }

    public async Task<Title?> GetTitleByIdAsync(int id)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        
        string query = """
                       select 
                       titles.id, 
                       name, 
                       creator, 
                       isbn,
                       total_chapters,
                       type_id,
                       total_pages,
                       total_volumes
                       from titles
                       left join books on titles.id = books.id
                       left join manga on titles.id = manga.id
                       where titles.id = @id
                       """;

        await using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }
        
        Title title = MapReaderToTitle(reader);
        return title;
    }

    public async Task<bool> AddTitleAsync(Title title)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();

        string titleQuery = """
                        insert into titles(name,creator, isbn, total_chapters, type_id)
                        values(@name, @creator, @isbn, @total_chapters, @type_id);
                        select last_insert_id();
                       """; // Inserts values in specified column
                            // last_insert_id() selects the last inserted id

        int typeId = title switch
        {
            Book => 1,
            Manga => 2,
            _ => throw new InvalidOperationException($"Unsupported title type: {title.TypeName}")
        };
                            
        await using MySqlCommand command = new MySqlCommand(titleQuery, connection); // Prepares the SQL command

        // Binds the shared title properties to SQL parameters to prevent SQL injection
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

        command.Parameters.AddWithValue("@type_id", typeId);

        // Executes the insert and retrieves the newly generated primary key from titles
        int newTitleId = Convert.ToInt32(await command.ExecuteScalarAsync());

        // Inserts type-specific properties into the matching child table using the new title ID
        if (title is Book book)
        {
            string bookQuery = """
                               insert into books (id, total_pages)
                               values (@id, @total_pages)
                               """;

            await using MySqlCommand bookCommand = new MySqlCommand(bookQuery, connection);
            bookCommand.Parameters.AddWithValue("@id", newTitleId);
            bookCommand.Parameters.AddWithValue("@total_pages", DBNull.Value);
            
            if (book.TotalPages != null)
            {
                bookCommand.Parameters["@total_pages"].Value = book.TotalPages;
            }

            await bookCommand.ExecuteNonQueryAsync(); // Executes the SQL commands
        } 
        else if (title is Manga manga)
        {
            string mangaQuery = """
                                insert into manga (id, total_volumes)
                                values (@id, @total_volumes)
                                """;

            await using MySqlCommand mangaCommand = new MySqlCommand(mangaQuery, connection);
            mangaCommand.Parameters.AddWithValue("@id", newTitleId);
            mangaCommand.Parameters.AddWithValue("@total_volumes", DBNull.Value);

            if (manga.TotalVolumes != null)
            {
                mangaCommand.Parameters["@total_volumes"].Value = manga.TotalVolumes;
            }

            await mangaCommand.ExecuteNonQueryAsync(); // executes the SQL commands
        }
        
        return newTitleId > 0;
    }

    public async Task<bool> DeleteTitleAsync(int id)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        
        string query = """
                       delete from titles
                       where id = @id
                       """;

        await using MySqlCommand command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateTitleAsync(Title title)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        
        string titleQuery = """
                       update titles
                       set name = @name,
                           creator = @creator,
                           isbn = @isbn,
                           total_chapters = @total_chapters
                           where id = @id
                       """;

        await using MySqlCommand command = new MySqlCommand(titleQuery, connection);

        command.Parameters.AddWithValue("@id", title.Id);
        command.Parameters.AddWithValue("@name", title.Name);
        command.Parameters.AddWithValue("@creator", title.Creator);
        
        command.Parameters.AddWithValue("@isbn", DBNull.Value);
        if (title.Isbn != null)
        {
            command.Parameters["@isbn"].Value = title.Isbn;
        }

        command.Parameters.AddWithValue("@total_chapters", DBNull.Value);
        if (title.TotalChapters != null)
        {
            command.Parameters["@total_chapters"].Value = title.TotalChapters;
        }

        if (title is Book book)
        {
            string bookQuery = """
                               update books
                               set total_pages = @total_pages
                               where id = @id
                               """;

            await using MySqlCommand bookCommand = new MySqlCommand(bookQuery, connection);

            bookCommand.Parameters.AddWithValue("@id", title.Id);
            bookCommand.Parameters.AddWithValue("@total_pages", DBNull.Value);
            
            if (book.TotalPages != null)
            {
                bookCommand.Parameters["@total_pages"].Value = book.TotalPages;
            }

            await bookCommand.ExecuteNonQueryAsync();
        }
        else if (title is Manga manga)
        {
            string mangaQuery = """
                                update manga
                                set total_volumes = @total_volumes
                                where id = @id
                                """;

            await using MySqlCommand mangaCommand = new MySqlCommand(mangaQuery, connection);

            mangaCommand.Parameters.AddWithValue("@id", title.Id);
            mangaCommand.Parameters.AddWithValue("@total_volumes", DBNull.Value);

            if (manga.TotalVolumes != null)
            {
                mangaCommand.Parameters["@total_volumes"].Value = manga.TotalVolumes;
            }

            await mangaCommand.ExecuteNonQueryAsync();
        }
            
        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    private Title MapReaderToTitle(MySqlDataReader reader)
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

        int? totalPages = null;

        if (!reader.IsDBNull(reader.GetOrdinal("total_pages")))
        {
            totalPages = reader.GetInt32("total_pages");
        }

        int? totalVolumes = null;

        if (!reader.IsDBNull(reader.GetOrdinal("total_volumes")))
        {
            totalVolumes = reader.GetInt32("total_volumes");
        }

        int id = reader.GetInt32("id");
        string name = reader.GetString("name");
        string creator = reader.GetString("creator");
        int typeId = reader.GetInt32("type_id");

        Title title = typeId switch
        {
            1 => new Book
            {
                Id = id,
                Name = name,
                Creator = creator,
                Isbn = isbn,
                TotalChapters = totalChapters,
                TotalPages = totalPages
            },
            2 => new Manga
            {
                Id = id,
                Name = name,
                Creator = creator,
                Isbn = isbn,
                TotalChapters = totalChapters,
                TotalVolumes = totalVolumes
            },
            _ => throw new InvalidOperationException($"Unknown title type: {typeId}")
        };

        return title;
    }
}