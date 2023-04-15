using Microsoft.Extensions.Options;
using Miskato_Blog.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Miskato_Blog.Services
{
    public class PostsService
    {
        private readonly IMongoCollection<Post> _postsCollection;

        private BsonDocument LookupStage { get; } =
            new BsonDocument{
                            {
                                "$addFields", new BsonDocument{
                                    {
                                        "Comments", new BsonDocument{
                                            {"$filter", new BsonDocument{
                                                {"input","$Comments"},
                                                {"as","item"},
                                                {"cond",new BsonDocument{
                                                    {"$eq",new BsonArray{"$$item.ParentCommentId",BsonNull.Value}}
                                                }}
                                            }}
                                    }
                                    }
                                }
                            }
                        };

        public PostsService(IOptions<MiskatoBlogMongoSettings> miskatoBlogMongoSettings)
        {
            var mongoClient = new MongoClient(miskatoBlogMongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(miskatoBlogMongoSettings.Value.DatabaseName);
            _postsCollection = mongoDatabase.GetCollection<Post>(miskatoBlogMongoSettings.Value.PostCollectionName);
        }


        // public async Task<List<Post>> GetAsync() => await _postsCollection.Find(_ => true).ToListAsync();
        public async Task<List<Post>> GetAsync() => await
            _postsCollection
            .Aggregate()
            .Lookup("comments", "_id", "PostId", "Comments")
            .AppendStage<Post>(LookupStage)
            .ToListAsync();

        public async Task<long> Count() => await _postsCollection.CountDocumentsAsync(_ => true);

        public async Task<Post?> GetAsync(string id) => await _postsCollection.Aggregate().Match(p => p.Id == id).AppendStage<Post>(LookupStage).FirstOrDefaultAsync();
        public async Task<List<Post>> GetAsync(PipelineDefinition<Post, Post> pipeline) => await
            _postsCollection.
            Aggregate(pipeline)
            .ToListAsync();

        public async Task InsertAsync(Post post) => await _postsCollection.InsertOneAsync(post);
        public async Task UpdateAsync(string id, Post post) => await _postsCollection.FindOneAndReplaceAsync(p => p.Id == id, post);
        public async Task RemoveAsync(string id) => await _postsCollection.DeleteOneAsync(p => p.Id == id);



    }
}

//Aggregation pipeline for Post
// [
//   {
//     $lookup:
//       /**
//        * from: The target collection.
//        * localField: The local join field.
//        * foreignField: The target join field.
//        * as: The name for the results.
//        * pipeline: Optional pipeline to run on the foreign collection.
//        * let: Optional variables to use in the pipeline field stages.
//        */
//       {
//         from: "comments",
//         localField: "_id",
//         foreignField: "PostId",
//         as: "Comments",
//       },
//   },
//   {
//     $addFields: {
//       Comments: {
//         $filter: {
//           input: "$Comments",
//           as: "item",
//           cond: {
//             $eq: ["$$item.ParentCommentId", null],
//           },
//         },
//       },
//     },
//   },
// ]

// [
//   {
//     $match:
//       /**
//        * query: The query in MQL.
//        */
//       {
//         _id: ObjectId("64326749841871dea8c7df7f"),
//       },
//   },
//   {
//     $lookup:
//       /**
//        * from: The target collection.
//        * localField: The local join field.
//        * foreignField: The target join field.
//        * as: The name for the results.
//        * pipeline: Optional pipeline to run on the foreign collection.
//        * let: Optional variables to use in the pipeline field stages.
//        */
//       {
//         from: "comments",
//         localField: "_id",
//         foreignField: "PostId",
//         as: "Comments",
//       },
//   },
//   {
//     $addFields: {
//       Comments: {
//         $filter: {
//           input: "$Comments",
//           as: "item",
//           cond: {
//             $eq: ["$$item.ParentCommentId", null],
//           },
//         },
//       },
//     },
//   },
// ]