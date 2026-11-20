using library_app.Models;
using library_app.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace library_app.Pages.Titles;

public class Create : PageModel
{
    private readonly ITitleRepository _titleRepository;

    [BindProperty] // Puts the form inside the property
    public Title NewTitle { get; set; } = new Title();

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
        
        await _titleRepository.AddTitleAsync(NewTitle);
        TempData["SuccessMessage"] = $"'{NewTitle.Name}' was successfully added";
        return RedirectToPage("/Index"); // Redirects users to the index page
    }
}