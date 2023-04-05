using Microsoft.Extensions.Options;
using Miskato_Blog.Models;
using MongoDB.Driver;

namespace Miskato_Blog.Services
{
    public class PostsService
    {
        private readonly IMongoCollection<Post> _postsCollection;

        public PostsService(IOptions<MiskatoBlogMongoSettings> miskatoBlogMongoSettings)
        {
            var mongoClient = new MongoClient(miskatoBlogMongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(miskatoBlogMongoSettings.Value.DatabaseName);
            _postsCollection = mongoDatabase.GetCollection<Post>(miskatoBlogMongoSettings.Value.PostCollectionName);
        }

        public async Task<List<Post>> GetAsync() => await _postsCollection.Find(_ => true).ToListAsync();

        public async Task<Post?> GetAsync(string id) => await _postsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        public async Task InsertAsync(Post post) => await _postsCollection.InsertOneAsync(post);
        public async Task UpdateAsync(string id, Post post) => await _postsCollection.FindOneAndReplaceAsync(p => p.Id == id, post);
        public async Task RemoveAsync(string id) => await _postsCollection.DeleteOneAsync(p => p.Id == id);


    }
}