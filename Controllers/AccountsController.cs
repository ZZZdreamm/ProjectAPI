using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using ProjectAPI.Helpers;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using UserProfile = ProjectAPI.Entities.UserProfile;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;

        public AccountsController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        //[HttpGet("listUsers")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        //public async Task<ActionResult<List<UserDTO>>> GetListUsers([FromQuery] PaginationDTO paginationDTO)
        //{
        //    var queryable = context.Users.AsQueryable();
        //    await HttpContext.InsertParametersPaginationInHeader(queryable);
        //    var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDTO).ToListAsync();
        //    return mapper.Map<List<UserDTO>>(users);
        //}


        //[HttpPost("makeAdmin")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        //public async Task<ActionResult> MakeAdmin([FromBody] string userId)
        //{
        //    var user = await userManager.FindByIdAsync(userId);
        //    await userManager.AddClaimAsync(user, new Claim("role", "admin"));
        //    return NoContent();
        //}

        //[HttpPost("removeAdmin")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        //public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
        //{
        //    var user = await userManager.FindByIdAsync(userId);
        //    await userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
        //    return NoContent();
        //}

        [HttpPost("profileImage")]
        public async Task<ActionResult> AddProfileImage([FromForm] ProfileAddImageDTO profileLogged)
        {
            var profile = context.Profiles.FirstOrDefault(x => x.Email == profileLogged.Email);

            profile.ProfileImage = await fileStorageService.SaveFile("profiles", profileLogged.ProfileImage);
            context.Profiles.Update(profile);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponse>> Create([FromBody] UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            var userNames = user.UserName.Split(" ");
            UserProfile profile = new UserProfile();
            profile.Id = user.Id;
            profile.Email = userCredentials.Email;

            context.Profiles.Add(profile);
            try
            {
                await context.SaveChangesAsync();
            }
            catch
            {

            }
         

            if (result.Succeeded)
            {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpPost("loginProfile/{userEmail}")]
        public async Task<ActionResult<UserProfile>> LoginProfile([FromRoute] string userEmail)
        {
            var profile = context.Profiles.FirstOrDefault(x => x.Email == userEmail);
            return profile;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var iClaims = await userManager.GetClaimsAsync(user);

            if (result.Succeeded)
            {
                return BuildToken(userCredentials, iClaims);
            }
            else
            {
                return BadRequest("Incorrect Login");
            }
        }

        //[HttpGet("userFriends/{profileEmail}")]
        //public async Task<ActionResult<List<UserProfile>>> GetUserFriends([FromRoute] string profileEmail)
        //{
        //    var profile = context.Profiles.FirstOrDefault(x => x.Email == profileEmail);
        //    var profileFriends = await context.ProfilesFriends.Where(x => x.ProfileId == profile.Id).ToListAsync();
        //    //var queryable = context.Friends
        //    //    .Include(x => x.ProfilesFriends).ThenInclude(x => x.Profile)
        //    //    .Where(x => x.Id == profileFriends.FriendId).AsQueryable();
        //    //var friend = context.Friends
        //    //   .Include(x => x.ProfilesFriends).ThenInclude(x => x.Profile);



        //    //await HttpContext.InsertParametersPaginationInHeader(queryable);
        //    //var friends = await queryable.OrderBy(x => x.Id).Paginate(paginationDTO).ToListAsync();
        //    if (profileFriends.Any())
        //    {
        //        var friends = new List<UserProfile>();
        //        foreach (var profileFriend in profileFriends)
        //        {
        //            friends.Add(context.Profiles.FirstOrDefault(x => x.Id == profileFriend.FriendId));
        //        }

        //        return mapper.Map<List<UserProfile>>(friends);
        //    }
        //    return NoContent();
        //}

        //[HttpPost("addFriend/{profileId}/{friendId}")]
        //public async Task<ActionResult> AddFriend([FromRoute] string profileId, [FromRoute] string friendId)
        //{
        //    var friend = await context.Profiles.FirstOrDefaultAsync(x => x.Id == friendId);
        //    var profile = await context.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
        //    var profileFriends = new ProfilesFriends{ProfileId = profileId, FriendId = friendId};
        //    context.Add(profileFriends);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("removeFriend/{profileId}/{friendId}")]
        //public async Task<ActionResult> DeleteFriend([FromRoute] string profileId, [FromRoute] string friendId)
        //{
        //    var friend = await context.ProfilesFriends.FirstOrDefaultAsync(x => x.FriendId == friendId && x.ProfileId == profileId);
        //    context.Remove(friend);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpGet("searchByName/{searchTerm}/{userEmail}")]
        public async Task<ActionResult<List<UserProfile>>> SearchProfiles([FromRoute] string searchTerm, [FromRoute] string userEmail)
        {
            var profiles = await context.Profiles.Where(x => x.Email.Contains(searchTerm) && x.Email != userEmail).Take(5).ToListAsync();

            return profiles;
        }



        //    [HttpGet("userFriends/{autorName}")]
        //public async Task<List<PostDTO>> GetUserPosts([FromQuery] PaginationDTO paginationDTO, [FromRoute] string autorName)
        //{
        //    var queryable = context.Posts.Where(x => x.AutorName == autorName).AsQueryable();
        //    await HttpContext.InsertParametersPaginationInHeader(queryable);
        //    var posts = await queryable.OrderBy(x => x.Id).Paginate(paginationDTO).ToListAsync();
        //    return mapper.Map<List<PostDTO>>(posts);
        //}

        private AuthenticationResponse BuildToken(UserCredentials userCredentials, IList<Claim> iClaims = null)
        {
            var claims = new List<Claim>();
            if (iClaims != null)
            {
                claims = new List<Claim>(iClaims);
                claims.Add(new Claim("email", userCredentials.Email));
            }
            else
            {
                claims = new List<Claim>()
                {
                     new Claim("email", userCredentials.Email),
                };
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationTime = expiration
            };
        }


    }
}
