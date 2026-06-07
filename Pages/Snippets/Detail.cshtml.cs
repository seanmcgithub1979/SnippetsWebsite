using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnippetsWebsite.Models;
using SnippetsWebsite.Services;

namespace SnippetsWebsite.Pages.Snippets;

public class DetailModel : PageModel
{
    private readonly ISnippetRepository _repo;

    public DetailModel(ISnippetRepository repo) => _repo = repo;

    public Snippet? Snippet { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Snippet = await _repo.GetByIdAsync(id);
        return Snippet is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        await _repo.DeleteAsync(id);
        return RedirectToPage("/Index");
    }
}
