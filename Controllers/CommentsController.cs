using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using ProjectAPI.Helpers;
using System.ComponentModel;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController :ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;

        public CommentsController(ApplicationDBContext context,IMapper mapper,IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<CommentDTO>>> GetCommentsForPost(int id)
        {
            var comments = new List<Comment>();
            try
            {
                comments = await context.Comments.Where(x => x.PostId == id).OrderBy(x => x.Id).ToListAsync();
            }
            catch
            {
                return NoContent();
            }
            var commentsDTO = new List<CommentDTO>();
            foreach (var comment in comments)
            {
                var autor = context.Profiles.Where(x => x.Id == comment.AutorId).ToList();
                CommentDTO commentDTO = new CommentDTO(id: comment.Id, postId: comment.PostId, autorName: autor[0].Email,
                    autorProfileImage: autor[0].ProfileImage, textContent: comment.TextContent);
                commentsDTO.Add(commentDTO);
            }
            return commentsDTO;
        }

        [HttpPost("create")]
        public async Task<ActionResult> PutComment([FromForm] CommentCreationDTO commentCreationDTO)
        {
            var comment = mapper.Map<Comment>(commentCreationDTO);
            var post = context.Posts.FirstOrDefault(x => x.Id == comment.PostId);
            post.AmountOfComments += 1;
            context.Posts.Update(post);
            context.Comments.Add(comment);

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
