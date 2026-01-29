using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public int PostId { get; set; }

        [JsonIgnore]
        public BlogPost Post { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
