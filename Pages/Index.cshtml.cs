using Microsoft.AspNetCore.Mvc.RazorPages;
using SnippetsWebsite.Models;
using SnippetsWebsite.Services;

namespace SnippetsWebsite.Pages;

public class IndexModel : PageModel
{
    private readonly ISnippetRepository _repo;

    public IndexModel(ISnippetRepository repo) => _repo = repo;

    public List<Snippet> Snippets { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public List<string> AllTags { get; set; } = [];

    public string? Query => Request.Query["q"];
    public string? Language => Request.Query["language"];
    public string? Tag => Request.Query["tag"];

    public async Task OnGetAsync()
    {
        var all = await _repo.GetAllAsync();
        Languages = all.Select(s => s.Language).Where(l => !string.IsNullOrEmpty(l)).Distinct().OrderBy(l => l).ToList();
        AllTags = all.SelectMany(s => s.Tags).Distinct().OrderBy(t => t).ToList();
        Snippets = await _repo.SearchAsync(Query, Language, Tag);
    }
}
