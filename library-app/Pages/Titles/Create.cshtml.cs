using library_app.Models.Requests;
using Logic.Interfaces;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Create : PageModel
{
    private readonly ITitleService _titleService;

    public Create(ITitleService titleService)
    {
        _titleService = titleService;
    }
    
    [BindProperty] // Puts the form inside the property
    public CreateTitleRequest TitleRequest { get; set; } = new CreateTitleRequest();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) // Checks if the values are correct
        {
            return Page(); // Keeps users on the same page
        }

        Title selectedType = TitleRequest.SelectedType switch
        {
            "Book" => new Book
            {
                Name = TitleRequest.Name,
                Creator = TitleRequest.Creator,
                Isbn = TitleRequest.Isbn,
                TotalChapters = TitleRequest.TotalChapters,
                TotalPages = TitleRequest.TotalPages
            },
            "Manga" => new Manga
            {
                Name = TitleRequest.Name,
                Creator = TitleRequest.Creator,
                Isbn = TitleRequest.Isbn,
                TotalChapters = TitleRequest.TotalChapters,
                TotalVolumes = TitleRequest.TotalVolumes
            },
            _ => throw new InvalidOperationException($"Unknown type")
        };
        
        await _titleService.AddTitleAsync(selectedType);
        TempData["SuccessMessage"] = $"'{selectedType.Name}' was successfully added";
        return RedirectToPage("/Index"); // Redirects users to the index page
    }
}