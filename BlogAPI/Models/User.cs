using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        [JsonIgnore]
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();

        [JsonIgnore]
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
