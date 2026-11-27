using library_app.Models.Requests;
using Logic.Interfaces;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Edit : PageModel
{
    private readonly ITitleService _titleService;
    
    public Edit(ITitleService titleService)
    {
        _titleService = titleService;
    }

    [BindProperty] // Puts the form inside the property
    public UpdateTitleRequest TitleRequest { get; set; } = new UpdateTitleRequest();
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Title? title = await _titleService.GetTitleByIdAsync(id);

        if (title == null)
        {
            return RedirectToPage("/Index");
        }

        TitleRequest.Id = title.Id;
        TitleRequest.Name = title.Name;
        TitleRequest.Creator = title.Creator;
        TitleRequest.Isbn = title.Isbn;
        TitleRequest.TotalChapters = title.TotalChapters;

        if (title is Book book)
        {
            TitleRequest.SelectedType = "Book";
            TitleRequest.TotalPages = book.TotalPages;
        }
        else if (title is Manga manga)
        {
            TitleRequest.SelectedType = "Manga";
            TitleRequest.TotalVolumes = manga.TotalVolumes;
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Title updateTitle = TitleRequest.SelectedType switch
        {
            "Book" => new Book
            {
                Id = TitleRequest.Id,
                Name = TitleRequest.Name,
                Creator = TitleRequest.Creator,
                Isbn = TitleRequest.Isbn,
                TotalChapters = TitleRequest.TotalChapters,
                TotalPages = TitleRequest.TotalPages
            },
            "Manga" => new Manga
            {
                Id = TitleRequest.Id,
                Name = TitleRequest.Name,
                Creator = TitleRequest.Creator,
                Isbn = TitleRequest.Isbn,
                TotalChapters = TitleRequest.TotalChapters,
                TotalVolumes = TitleRequest.TotalVolumes
            },
            _ => throw new InvalidOperationException("Unknown type")
        };
        
        await _titleService.UpdateTitleAsync(updateTitle);
        TempData["SuccessMessage"] = $"'{updateTitle.Name}' was successfully edited";

        return RedirectToPage("/Index");
    }
}