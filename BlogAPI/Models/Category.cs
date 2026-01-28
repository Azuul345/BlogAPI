namespace BlogAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BlogPost> Posts {  get; set; } = new List<BlogPost>();
    }
}
