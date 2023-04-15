using Microsoft.AspNetCore.Mvc;
using Miskato_Blog.Models;
using Miskato_Blog.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Miskato_Blog.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly PostsService _postsService;

        public PostsController(PostsService postsService)
        {
            _postsService = postsService;
        }

        [HttpGet("/all")]
        public async Task<List<Post>> Get() => await _postsService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Post>> Get(string id)
        {
            var foundPost = await _postsService.GetAsync(id);
            if (foundPost is null)
                return NotFound();
            return foundPost;
        }
        /// <summary>
        /// Endpoint to filter the post according certain keywords and filter them by their creation date
        /// </summary>
        /// <param name="keywords">The keyword string, seprating each keyword by a comma ","</param>
        /// <param name="order">The order in which the results should be sorted. true for ascending, false for descending</param>
        /// <param name="currentPage">The current page of the pagination. Default value 1</param>
        /// <returns>A list of post that match the specified criteria (keywords)</returns>
        [HttpGet]
        public async Task<List<Post>> Get(string keywords, bool order, int currentPage = 1)
        {
            //The amount of documents per page
            long Limit = 2;
            //Calculate the offset to skip documents in order to implement pagination
            long Offset = (currentPage - 1) * Limit;

            //Pipeline construction through "PipelineDefinition" class.
            var sort = Builders<Post>.Sort;
            var MatchKeywords = Builders<Post>.Filter.In("Keywords", keywords.Split(","));
            PipelineDefinition<Post, Post> pipeline = new EmptyPipelineDefinition<Post>();

            //Use ternary to define whether we sort the results in ascending form or descending form
            pipeline = order ? pipeline.Sort(sort.Ascending(p => p.CreatedAt)) : pipeline.Sort(sort.Descending(p => p.CreatedAt));

            //Construct the rest of the pipeline to match the keywords array
            pipeline = pipeline.Match(MatchKeywords);
            //Implement pagination skipping the right amount of documents and limit the number of documents retrieved.
            pipeline = pipeline.Skip(Offset);
            pipeline = pipeline.Limit(Limit);

            //Execute the query to the database
            var posts = await _postsService.GetAsync(pipeline);

            return posts;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post newPost)
        {
            newPost.CreatedAt = DateTime.UtcNow;
            await _postsService.InsertAsync(newPost);
            return CreatedAtAction(nameof(Get), new { id = newPost.Id }, newPost);
        }

        [HttpPut("{id:length(24)}")]

        public async Task<IActionResult> Update(string id, Post newPost)
        {
            var foundPost = await _postsService.GetAsync(id);
            if (foundPost is null)
            {
                return NotFound();
            }
            newPost.Id = foundPost.Id;
            await _postsService.UpdateAsync(id, newPost);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var postFound = await _postsService.GetAsync(id);
            if (postFound is null)
            {
                return NotFound();
            }
            await _postsService.RemoveAsync(id);
            return NoContent();
        }

    }
}