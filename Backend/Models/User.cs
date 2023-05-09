using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Miskato_Blog.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ProfilePicture { get; set; } = null!;
        public string? Username { get; set; } = null!;


    }
}