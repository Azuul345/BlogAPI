using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<BlogPost> Posts {  get; set; } = new List<BlogPost>();
    }
}
