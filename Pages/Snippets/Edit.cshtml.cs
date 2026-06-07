using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnippetsWebsite.Services;

namespace SnippetsWebsite.Pages.Snippets;

public class EditModel : PageModel
{
    private readonly ISnippetRepository _repo;

    public EditModel(ISnippetRepository repo) => _repo = repo;

    [BindProperty(SupportsGet = true)]
    public string SnippetId { get; set; } = "";

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        SnippetId = id;
        var snippet = await _repo.GetByIdAsync(id);
        if (snippet is null) return NotFound();

        Input = new InputModel
        {
            Title = snippet.Title,
            Language = snippet.Language,
            Description = snippet.Description,
            TagsRaw = string.Join(", ", snippet.Tags),
            Code = snippet.Code
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        SnippetId = id;
        if (!ModelState.IsValid) return Page();

        var snippet = await _repo.GetByIdAsync(id);
        if (snippet is null) return NotFound();

        snippet.Title = Input.Title;
        snippet.Language = Input.Language;
        snippet.Description = Input.Description ?? "";
        snippet.Code = Input.Code;
        snippet.Tags = Input.TagsRaw?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? [];

        await _repo.UpdateAsync(snippet);
        return RedirectToPage("/Snippets/Detail", new { id });
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
