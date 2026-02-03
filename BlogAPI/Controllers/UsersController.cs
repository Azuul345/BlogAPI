using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Dtos;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // 1. Kolla om username redan finns
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (existingUser != null)
            {
                return BadRequest("Username is already taken.");
            }

            // 2. Skapa user
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                // För kursens skull kan du spara plaintext eller enkel hash om ni inte gått igenom hashing
                Password = request.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName
                                          && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Uppgiften: returnera userId som ska användas i andra anrop
            return Ok(new { userId = user.Id, user.UserName });
        }


        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //kolla att det är rätt ägare
            if (user.Id != request.UserId)
            {
                return Forbid("You are not allowed to edit this User.");
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Password = request.Password;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, [FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // bara ägaren får ta bort
            if (user.Id != userId)
            {
                return Forbid("You are not allowed to delete this user.");
            }

            // ta bort alla kommentarer skrivna av användaren
            var userComments = await _context.Comments
                .Where(c => c.UserId == id)
                .ToListAsync();

            _context.Comments.RemoveRange(userComments);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
