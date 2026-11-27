using library_app.Models;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Create : PageModel
{
    private readonly ITitleRepository _titleRepository;

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

    public Create(ITitleRepository titleRepository)
    {
        _titleRepository = titleRepository;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) // Checks if the values are correct
        {
            return Page(); // Keeps users on the same page
        }

        Title selectedType = SelectedType switch
        {
            "Book" => new Book
            {
                Name = Name,
                Creator = Creator,
                Isbn = Isbn,
                TotalChapters = TotalChapters,
                TotalPages = TotalPages
            },
            "Manga" => new Manga
            {
                Name = Name,
                Creator = Creator,
                Isbn = Isbn,
                TotalChapters = TotalChapters,
                TotalVolumes = TotalVolumes
            },
            _ => throw new InvalidOperationException($"Unknown type")
        };
        
        await _titleRepository.AddTitleAsync(selectedType);
        TempData["SuccessMessage"] = $"'{selectedType.Name}' was successfully added";
        return RedirectToPage("/Index"); // Redirects users to the index page
    }
}