using BlogAPI.Models;

namespace BlogAPI.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetForPostAsync(int postId);
        Task<Comment?> GetByIdAsync(int id);
        Task AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
