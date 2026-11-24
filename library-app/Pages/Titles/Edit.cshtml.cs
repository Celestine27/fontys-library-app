using library_app.Models;
using library_app.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Edit : PageModel
{
    private readonly ITitleRepository _titleRepository;

    [BindProperty] 
    public Title EditTitle { get; set; } = new Title();

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

        EditTitle = title;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        await _titleRepository.UpdateTitleAsync(EditTitle);
        TempData["SuccessMessage"] = $"'{EditTitle.Name}' was successfully edited";

        return RedirectToPage("/Index");
    }
}