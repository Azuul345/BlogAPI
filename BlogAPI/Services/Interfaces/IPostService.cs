using BlogAPI.Dtos;

namespace BlogAPI.Services.Interfaces
{
    public interface IPostService
    {
        Task<List<PostResponse>> GetAllAsync();
        Task<PostResponse?> GetByIdAsync(int id);
        Task<List<PostResponse>> SearchAsync(string? title, int? categoryId);
        Task<PostResponse> CreateAsync(CreatePostRequest request);
        Task<bool> UpdateAsync(int id, UpdatePostRequest request);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
