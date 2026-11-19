using library_app.Models;
using library_app.Repository;
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
}