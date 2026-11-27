using library_app.Models;

namespace Logic.Interfaces;

public interface ITitleRepository
{
    Task<List<Title>> GetAllTitlesAsync();

    Task<Title?> GetTitleByIdAsync(int id);

    Task<bool> AddTitleAsync(Title title);

    Task<bool> DeleteTitleAsync(int id);

    Task<bool> UpdateTitleAsync(Title title);
}