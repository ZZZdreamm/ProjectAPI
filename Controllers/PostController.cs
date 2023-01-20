using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using ProjectAPI.Helpers;
using System.ComponentModel;
using System.Linq;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private string container = "posts";

        public PostController(ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService )
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet("all")]
        public async Task<List<PostDTO>> GetAllPosts()
        {
            var posts = await context.Posts.ToListAsync();
            foreach (var post in posts)
            {
                var autor = context.Profiles.FirstOrDefault(x => x.Email == post.AutorName);
                post.AutorProfileImage = autor.ProfileImage;
            }
            return mapper.Map<List<PostDTO>>(posts);
        }

        [HttpGet("userPosts/{autorName}")]
        public async Task<List<PostDTO>> GetUserPosts([FromQuery] PaginationDTO paginationDTO, [FromRoute] string autorName)
        {
            var queryable = context.Posts.Where(x => x.AutorName == autorName).AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var posts = await queryable.OrderBy(x => x.Id).Paginate(paginationDTO).ToListAsync();
            foreach (var post in posts)
            {
                var autor = context.Profiles.FirstOrDefault(x => x.Email == post.AutorName);
                post.AutorProfileImage = autor.ProfileImage;
            }
            return mapper.Map<List<PostDTO>>(posts);
        }

        [HttpPost("post")]
        public async Task<ActionResult> Post([FromForm]PostCreationDTO postCreationDTO)
        {

            var post = mapper.Map<Post>(postCreationDTO);

            if (post.MediaFile != null)
            {
                 post.MediaFile = await fileStorageService.SaveFile(container, postCreationDTO.MediaFile);
            }
            context.Posts.Add(post);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("likes/{id}/{profileId}")]
        public async Task<ActionResult> PutLike(int id, string profileId)
        {
            var post = context.Posts.FirstOrDefault(x => x.Id == id);
            var like = new Like{ PostId = id,ProfileId = profileId};
            post.AmountOfLikes += 1;

            context.Posts.Update(post);
            context.Likes.Add(like);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("likes/{id}/{profileId}")]
        public async Task<ActionResult> DeleteLike(int id, string profileId)
        {
            var post = context.Posts.FirstOrDefault(x => x.Id == id);
            var like = context.Likes.FirstOrDefault(x => x.PostId == id && x.ProfileId == profileId);
            post.AmountOfLikes -= 1;

            context.Posts.Update(post);
            context.Likes.Remove(like);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("userLiked/{postId}/{profileId}")]
        public async Task<ActionResult<bool>> UserLiked(int postId, string profileId)
        {
            var like = await context.Likes.Where(x => x.ProfileId == profileId && x.PostId == postId).ToListAsync();
            var userLiked = false;
            if (like.Any())
            {
                userLiked = true;
            }
            return userLiked;
        }

        [HttpGet("filter")]
        public async Task<List<PostDTO>> FindPosts([FromBody] string searchName)
        {

            var posts = await context.Posts.Where(x => x.AutorName.Contains(searchName)).OrderBy(x => x.Id).ToListAsync();
            return mapper.Map<List<PostDTO>>(posts);
        }


        [HttpGet("getMore/{amount}")]

        public async Task<ActionResult<List<PostDTO>>> GetPosts([FromRoute] int amount = 1)
        {
            var posts = context.Posts.Take(amount*10).ToList();
            var postsDTOs = mapper.Map<List<PostDTO>>(posts);
            return postsDTOs;
        }
    }
}
