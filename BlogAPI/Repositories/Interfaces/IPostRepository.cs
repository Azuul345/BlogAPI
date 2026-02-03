using BlogAPI.Models;

namespace BlogAPI.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<List<BlogPost>> GetAllAsync();
        Task<BlogPost?> GetByIdAsync(int id);
        Task<List<BlogPost>> SearchAsync(string? title, int? categoryId);
        Task AddAsync(BlogPost post);
        Task UpdateAsync(BlogPost post);
        Task DeleteAsync(BlogPost post);
        Task<bool> ExistsAsync(int id);
    }
}
