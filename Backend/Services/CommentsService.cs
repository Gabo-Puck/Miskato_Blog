using Microsoft.Extensions.Options;
using Miskato_Blog.Models;

using MongoDB.Driver;

namespace Miskato_Blog.Services
{
    public class CommentsService
    {
        private readonly IMongoCollection<Comment> _commentCollection;

        public CommentsService(IOptions<MiskatoBlogMongoSettings> miskatoBlogMongoSettings)
        {
            var mongoClient = new MongoClient(miskatoBlogMongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(miskatoBlogMongoSettings.Value.DatabaseName);
            _commentCollection = mongoDatabase.GetCollection<Comment>(miskatoBlogMongoSettings.Value.CommentCollectionName);
        }

        public async Task<List<Comment>> GetAsync() => await _commentCollection.Find(_ => true).ToListAsync();

        public async Task<Comment?> GetAsync(string id) => await _commentCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task<List<Comment>> GetChildrenAsync(string id) => await _commentCollection.Find(c => c.ParentCommentId == id).ToListAsync();

        public async Task InsertAsync(Comment comment) => await _commentCollection.InsertOneAsync(comment);

        public async Task UpdateAsync(string id, Comment comment) => await _commentCollection.FindOneAndReplaceAsync(c => c.Id == id, comment);

        public async Task DeleteAsync(string id) => await _commentCollection.FindOneAndDeleteAsync(c => c.Id == id);
    }
}