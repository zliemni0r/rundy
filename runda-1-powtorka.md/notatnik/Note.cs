namespace notatnik;

public class Note
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public override string ToString() => string.IsNullOrWhiteSpace(Title) ? "(Bez tytułu)" : Title;
}
