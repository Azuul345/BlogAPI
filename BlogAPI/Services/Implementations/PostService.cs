using AutoMapper;
using BlogAPI.Data;
using BlogAPI.Dtos;
using BlogAPI.Models;
using BlogAPI.Repositories.Interfaces;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly AppDbContext _context;   // för user/category checks
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, AppDbContext context, IMapper mapper)
        {
            _postRepository = postRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PostResponse>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            return _mapper.Map<List<PostResponse>>(posts);
        }

        public async Task<PostResponse?> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return null;

            return _mapper.Map<PostResponse>(post);
        }

        public async Task<List<PostResponse>> SearchAsync(string? title, int? categoryId)
        {
            var posts = await _postRepository.SearchAsync(title, categoryId);
            return _mapper.Map<List<PostResponse>>(posts);
        }

        public async Task<PostResponse> CreateAsync(CreatePostRequest request)
        {
            // validera user och category här, istället för i controllern
            var user = await _context.Users.FindAsync(request.UserId)
                       ?? throw new ArgumentException("User not found.");

            var category = await _context.Categories.FindAsync(request.CategoryId)
                          ?? throw new ArgumentException("Category not found.");

            var post = new BlogPost
            {
                Title = request.Title,
                Text = request.Text,
                UserId = request.UserId,
                CategoryId = request.CategoryId
            };

            await _postRepository.AddAsync(post);

            // hämta tillbaka med navigation för att kunna mappa till DTO
            post = await _postRepository.GetByIdAsync(post.Id) ?? post;

            return _mapper.Map<PostResponse>(post);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePostRequest request)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return false;

            if (post.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Not allowed to edit this post.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Category not found.");
            }

            post.Title = request.Title;
            post.Text = request.Text;
            post.CategoryId = request.CategoryId;

            await _postRepository.UpdateAsync(post);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return false;

            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException("Not allowed to delete this post.");
            }

            await _postRepository.DeleteAsync(post);
            return true;
        }
    }
}
