using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController :ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        public ProfileController(ApplicationDBContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProfileDTO>> Get(int id)
        {
            var profile = context.Profiles.Where(x => x.Id == id);
            return mapper.Map<ProfileDTO>(profile);
        }


    }
}
