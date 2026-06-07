namespace SnippetsWebsite.Models;

public class Snippet
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "";
    public string Language { get; set; } = "";
    public string Description { get; set; } = "";
    public string Code { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
