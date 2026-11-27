using Logic.Interfaces;
using Logic.Models;

namespace Logic.Services;

public class TitleService : ITitleService
{
    private readonly ITitleRepository _titleRepository;

    public TitleService(ITitleRepository titleRepository)
    {
        _titleRepository = titleRepository;
    }
    
    public async Task<List<Title>> GetAllTitlesAsync()
    {
        List<Title> titles = await _titleRepository.GetAllTitlesAsync();

        return titles;
    }

    public async Task<Title?> GetTitleByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id cannot be 0 or negative.");
        }

        return await _titleRepository.GetTitleByIdAsync(id);
    }

    public async Task<bool> AddTitleAsync(Title title)
    {
        ValidateTitle(title);
        
        return await _titleRepository.AddTitleAsync(title);
    }

    public async Task<bool> DeleteTitleAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id cannot be 0 or negative.");
        }

        return await _titleRepository.DeleteTitleAsync(id);
    }

    public async Task<bool> UpdateTitleAsync(Title title)
    {
        ValidateTitle(title);

        if (title.Id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(title.Id), "Id cannot be 0 or negative.");
        }

        return await _titleRepository.UpdateTitleAsync(title);
    }

    private void ValidateTitle(Title title)
    {
        ArgumentNullException.ThrowIfNull(title); // Checks if title object is null
        
        // Checks if name and creator are null or has whitespaces
        if (string.IsNullOrWhiteSpace(title.Name) || string.IsNullOrWhiteSpace(title.Creator)) 
        {
            throw new ArgumentException("Name and Creator are mandatory");
        }

        if (title is Book book)
        {
            if (book.TotalPages < 0) // Checks if total pages is lower than 0
            {
                throw new ArgumentOutOfRangeException(nameof(book.TotalPages), "Total pages cannot be negative.");
            }
        }

        if (title is Manga manga)
        {
            if (manga.TotalVolumes < 0) // Checks if total volumes is lower than 0
            {
                throw new ArgumentOutOfRangeException(nameof(manga.TotalVolumes), "Total volumes cannot be negative");
            }
        }   
    }
}