using Microsoft.AspNetCore.Mvc;
using Miskato_Blog.Models;
using Miskato_Blog.Services;

namespace Miskato_Blog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentsService _commentsService;
        private readonly PostsService _postsService;

        public CommentController(CommentsService commentsService, PostsService postsService)
        {
            _commentsService = commentsService;
            _postsService = postsService;
        }

        [HttpGet]
        public async Task<List<Comment>> Get() => await _commentsService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Comment>> Get(string id)
        {
            var comment = await _commentsService.GetAsync(id);
            if (comment is null)
                return NotFound();
            return comment;
        }

        [HttpGet("{id:length(24)}/children")]
        public async Task<List<Comment>> GetChildren(string id) => await _commentsService.GetChildrenAsync(id);

        [HttpPost]
        public async Task<IActionResult> Post(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            var post = await _postsService.GetAsync(comment.PostId);
            if (post is null)
                return NotFound();

            await _commentsService.InsertAsync(comment);
            return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }


        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Comment comment)
        {
            var commentFound = await _commentsService.GetAsync(id);
            if (commentFound is null)
                return NotFound();
            await _commentsService.UpdateAsync(id, comment);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var commentFound = await _commentsService.GetAsync(id);
            if (commentFound is null)
                return NotFound();
            await _commentsService.DeleteAsync(id);
            return NoContent();
        }


    }
}