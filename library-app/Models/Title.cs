namespace library_app.Models;

public class Title
{
    public int Id { get; set; }
    public required  string Name { get; set; }
    public required string Creator { get; set; }
    public string? IsbnNumber { get; set; }
    public int MaxChapters { get; set; }
}