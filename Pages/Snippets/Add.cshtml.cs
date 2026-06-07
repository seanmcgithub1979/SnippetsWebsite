using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnippetsWebsite.Models;
using SnippetsWebsite.Services;

namespace SnippetsWebsite.Pages.Snippets;

public class AddModel : PageModel
{
    private readonly ISnippetRepository _repo;

    public AddModel(ISnippetRepository repo) => _repo = repo;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var snippet = new Snippet
        {
            Title = Input.Title,
            Language = Input.Language,
            Description = Input.Description ?? "",
            Code = Input.Code,
            Tags = Input.TagsRaw?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? []
        };

        await _repo.AddAsync(snippet);
        return RedirectToPage("/Snippets/Detail", new { id = snippet.Id });
    }

    public class InputModel
    {
        [Required] public string Title { get; set; } = "";
        [Required] public string Language { get; set; } = "";
        public string? Description { get; set; }
        public string? TagsRaw { get; set; }
        [Required] public string Code { get; set; } = "";
    }
}
