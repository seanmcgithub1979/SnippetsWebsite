using SnippetsWebsite.Models;

namespace SnippetsWebsite.Services;

public interface ISnippetRepository
{
    Task<List<Snippet>> GetAllAsync();
    Task<Snippet?> GetByIdAsync(string id);
    Task AddAsync(Snippet snippet);
    Task UpdateAsync(Snippet snippet);
    Task DeleteAsync(string id);
    Task<List<Snippet>> SearchAsync(string? query, string? language, string? tag);
}
