using System.Text.Json;
using SnippetsWebsite.Models;

namespace SnippetsWebsite.Services;

public class JsonSnippetRepository : ISnippetRepository
{
    private readonly string _filePath;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public JsonSnippetRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "snippets.json");
    }

    public async Task<List<Snippet>> GetAllAsync()
    {
        return await ReadAsync();
    }

    public async Task<Snippet?> GetByIdAsync(string id)
    {
        var all = await ReadAsync();
        return all.FirstOrDefault(s => s.Id == id);
    }

    public async Task AddAsync(Snippet snippet)
    {
        await _lock.WaitAsync();
        try
        {
            var all = await ReadAsync();
            all.Add(snippet);
            await WriteAsync(all);
        }
        finally { _lock.Release(); }
    }

    public async Task UpdateAsync(Snippet snippet)
    {
        await _lock.WaitAsync();
        try
        {
            var all = await ReadAsync();
            var idx = all.FindIndex(s => s.Id == snippet.Id);
            if (idx >= 0)
            {
                snippet.UpdatedAt = DateTime.UtcNow;
                all[idx] = snippet;
                await WriteAsync(all);
            }
        }
        finally { _lock.Release(); }
    }

    public async Task DeleteAsync(string id)
    {
        await _lock.WaitAsync();
        try
        {
            var all = await ReadAsync();
            all.RemoveAll(s => s.Id == id);
            await WriteAsync(all);
        }
        finally { _lock.Release(); }
    }

    public async Task<List<Snippet>> SearchAsync(string? query, string? language, string? tag)
    {
        var all = await ReadAsync();

        if (!string.IsNullOrWhiteSpace(language))
            all = all.Where(s => s.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(tag))
            all = all.Where(s => s.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            all = all.Where(s =>
                s.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                s.Code.Contains(q, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        return all.OrderByDescending(s => s.UpdatedAt).ToList();
    }

    private async Task<List<Snippet>> ReadAsync()
    {
        if (!File.Exists(_filePath)) return [];
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Snippet>>(json, _json) ?? [];
    }

    private async Task WriteAsync(List<Snippet> snippets)
    {
        var json = JsonSerializer.Serialize(snippets, _json);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
