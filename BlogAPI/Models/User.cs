namespace BlogAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
