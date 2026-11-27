using Logic.Interfaces;
using Logic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages;

public class IndexModel : PageModel
{
    private readonly ITitleService _titleService;

    public List<Title> Titles { get; set; } = [];
    
    public IndexModel(ITitleService titleService)
    {
        _titleService = titleService;
    }
    
    public async Task OnGetAsync()
    {
        Titles = await _titleService.GetAllTitlesAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        Title? title = await _titleService.GetTitleByIdAsync(id);
        
        if (title == null)
        {
            return RedirectToPage();
        }
        
        bool isDeleted = await _titleService.DeleteTitleAsync(id);

        if (isDeleted)
        {
            TempData["SuccessMessage"] = $"'{title.Name}' was successfully deleted";
        }

        return RedirectToPage();
    }
}