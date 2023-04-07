using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Miskato_Blog.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = null!;

        public string HtmlContent { get; set; } = null!;

        public string[] Keywords { get; set; } = null!;

        public User? Author { get; set; }

        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>()
;
        public Post()
        {
            Comments = new List<Comment>();
        }

    }
}