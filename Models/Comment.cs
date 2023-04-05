using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Miskato_Blog.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("Content")]
        public string HtmlContent { get; set; } = null!;

        public Comment[] Comments { get; set; } = null!;
    }
}