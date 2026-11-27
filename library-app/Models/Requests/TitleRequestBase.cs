using System.ComponentModel.DataAnnotations;

namespace library_app.Models.Requests;

public abstract class TitleRequestBase
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Creator is required")]
    public string Creator { get; set; } = string.Empty;
    
    public string? Isbn { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Total chapters cannot be negative")]
    public int? TotalChapters { get; set; }
    
    [Required(ErrorMessage = "Please select a type")]
    public string SelectedType { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Total pages cannot be negative")]
    public int? TotalPages { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Total volumes cannot be negative")]
    public int? TotalVolumes { get; set; }
}