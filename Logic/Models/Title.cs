namespace Logic.Models;

public abstract class Title
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Creator { get; set; }
    public string? Isbn { get; set; }
    public int? TotalChapters { get; set; }
    public string TypeName => GetType().Name;
}