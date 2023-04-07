using Microsoft.AspNetCore.Mvc;
using Miskato_Blog.Models;
using Miskato_Blog.Services;

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

        [HttpGet]
        public async Task<List<Post>> Get() => await _postsService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Post>> Get(string id)
        {
            var post = await _postsService.GetAsync(id);
            if (post is null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post newPost)
        {
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