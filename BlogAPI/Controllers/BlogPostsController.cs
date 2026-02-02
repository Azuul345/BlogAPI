using AutoMapper;
using BlogAPI.Dtos;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostResponse>>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .ToListAsync();

            var response = _mapper.Map<List<PostResponse>>(posts);

            return Ok(response);
        }



        // GET: api/posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponse>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<PostResponse>(post);

            return Ok(response);
        }

        // POST: api/posts
        [HttpPost]
        public async Task<ActionResult<BlogPost>> CreatePost(CreatePostRequest request)
        {
            // 1. Kontrollera att user finns
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // 2. Kontrollera att category finns
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return BadRequest("Category not found.");
            }

            // 3. Skapa post
            var post = new BlogPost
            {
                Title = request.Title,
                Text = request.Text,
                UserId = request.UserId,
                CategoryId = request.CategoryId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }



        // PUT: api/posts/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostRequest request)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // kolla att det är rätt ägare
            if (post.UserId != request.UserId)
            {
                return Forbid("You are not allowed to edit this post.");
            }

            // kolla att kategori finns
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category not found.");
            }

            post.Title = request.Title;
            post.Text = request.Text;
            post.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/posts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] int userId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // bara ägaren får ta bort
            if (post.UserId != userId)
            {
                return Forbid("You are not allowed to delete this post.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/posts/1/comments
        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForPost(int postId)
        {
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
            {
                return NotFound("Post not found.");
            }

            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync();

            return Ok(comments);
        }

        // POST: api/posts/1/comments
        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<Comment>> CreateComment(int postId, CreateCommentRequest request)
        {
            // 1. Kolla att post finns
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // 2. Kolla att user finns
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // 3. Logik: man får inte kommentera sitt eget inlägg
            if (post.UserId == request.UserId)
            {
                return Forbid("You cannot comment on your own post.");
            }

            // 4. Skapa kommentar
            var comment = new Comment
            {
                Text = request.Text,
                PostId = postId,
                UserId = request.UserId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentsForPost), new { postId = postId }, comment);
        }


        // GET: api/posts/search?title=...&categoryId=...
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> SearchPosts(
            [FromQuery] string? title,
            [FromQuery] int? categoryId)
        {
            var query = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var result = await query.ToListAsync();

            return Ok(result);
        }


    }
}
