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
        public string HtmlContent { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ParentCommentId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; } = null!;

        public DateTime CreatedAt { get; set; }


    }

}