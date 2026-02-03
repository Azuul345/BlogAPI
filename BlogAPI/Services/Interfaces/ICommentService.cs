using BlogAPI.Dtos;

namespace BlogAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentResponse>?> GetForPostAsync(int postId);
        Task<CommentResponse> CreateAsync(int postId, CreateCommentRequest request);
    }
}
