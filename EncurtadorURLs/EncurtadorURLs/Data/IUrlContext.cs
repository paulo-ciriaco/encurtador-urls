using EncurtadorURLs.Models;

namespace EncurtadorURLs.Data;

public interface IUrlContext
{
    Task<bool> ShortCodeExistsAsync(string shortCode);

    Task AddAsync(UrlItem item);

    Task<UrlItem?> GetByIdAsync(Guid id);
}