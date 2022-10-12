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
        public async Task<List<PostDTO>> GetPosts([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Posts.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var posts = await queryable.OrderBy(x => x.Id).Paginate(paginationDTO).ToListAsync();
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
            if (post.AutorProfileImage != null)
            {
                post.AutorProfileImage = await fileStorageService.SaveFile("profiles", postCreationDTO.AutorProfileImage);
            }
            context.Posts.Add(post);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<List<PostDTO>> FindPosts([FromBody] string searchName)
        {

            var posts = await context.Posts.Where(x => x.AutorName.Contains(searchName)).OrderBy(x => x.Id).ToListAsync();
            return mapper.Map<List<PostDTO>>(posts);
        }
    }
}
