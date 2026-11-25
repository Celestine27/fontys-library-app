using library_app.Models;
using library_app.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Edit : PageModel
{
    private readonly ITitleRepository _titleRepository;
    
    [BindProperty]
    public int Id { get; set; }
    
    [BindProperty] // Puts the form inside the property
    public string Name { get; set; } = string.Empty;
    
    [BindProperty] 
    public string Creator { get; set; } = string.Empty;
    
    [BindProperty] 
    public string? Isbn { get; set; }
    
    [BindProperty] 
    public int? TotalChapters { get; set; }

    [BindProperty] 
    public string SelectedType { get; set; } = string.Empty;

    [BindProperty]
    public int? TotalPages { get; set; }
    
    [BindProperty]
    public int? TotalVolumes { get; set; }

    public Edit(ITitleRepository titleRepository)
    {
        _titleRepository = titleRepository;
    }
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Title? title = await _titleRepository.GetTitleByIdAsync(id);

        if (title == null)
        {
            return RedirectToPage("/Index");
        }

        Id = title.Id;
        Name = title.Name;
        Creator = title.Creator;
        Isbn = title.Isbn;
        TotalChapters = title.TotalChapters;

        if (title is Book book)
        {
            SelectedType = "Book";
            TotalPages = book.TotalPages;
        }
        else if (title is Manga manga)
        {
            SelectedType = "Manga";
            TotalVolumes = manga.TotalVolumes;
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Title updateTitle = SelectedType switch
        {
            "Book" => new Book
            {
                Id = Id,
                Name = Name,
                Creator = Creator,
                Isbn = Isbn,
                TotalChapters = TotalChapters,
                TotalPages = TotalPages
            },
            "Manga" => new Manga
            {
                Id = Id,
                Name = Name,
                Creator = Creator,
                Isbn = Isbn,
                TotalChapters = TotalChapters,
                TotalVolumes = TotalVolumes
            },
            _ => throw new InvalidOperationException("Unknown type")
        };
        
        await _titleRepository.UpdateTitleAsync(updateTitle);
        TempData["SuccessMessage"] = $"'{updateTitle.Name}' was successfully edited";

        return RedirectToPage("/Index");
    }
}