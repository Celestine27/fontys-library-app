using library_app.Models;

namespace library_app.Repository;

public interface ITitleRepository
{
    Task<List<Title>> GetAllTitlesAsync();

    Task<bool> AddTitleAsync(Title title);
}