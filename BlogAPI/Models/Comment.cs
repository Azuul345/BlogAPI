namespace BlogAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public int PostId { get; set; }
        public BlogPost Post { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
