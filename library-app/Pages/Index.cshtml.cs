using library_app.Models;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages;

public class IndexModel : PageModel
{
    private readonly ITitleRepository _titleRepository;

    public List<Title> Titles { get; set; } = [];
    
    public IndexModel(ITitleRepository titleRepository)
    {
        _titleRepository = titleRepository;
    }
    
    public async Task OnGetAsync()
    {
        Titles = await _titleRepository.GetAllTitlesAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        Title? title = await _titleRepository.GetTitleByIdAsync(id);
        
        if (title == null)
        {
            return RedirectToPage();
        }
        
        bool isDeleted = await _titleRepository.DeleteTitleAsync(id);

        if (isDeleted)
        {
            TempData["SuccessMessage"] = $"'{title.Name}' was successfully deleted";
        }

        return RedirectToPage();
    }
}