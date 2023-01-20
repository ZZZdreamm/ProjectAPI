using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using ProjectAPI.Helpers;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/friends")]
    public class FriendsController:ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;

        public FriendsController(UserManager<IdentityUser> userManager,
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

        [HttpGet("userFriends/{profileEmail}")]
        public async Task<ActionResult<List<UserProfile>>> GetUserFriends([FromRoute] string profileEmail)
        {
            var profile = context.Profiles.FirstOrDefault(x => x.Email == profileEmail);
            var profileFriends = await context.ProfilesFriends.Where(x => x.ProfileId == profile.Id).ToListAsync();
            //var queryable = context.Friends
            //    .Include(x => x.ProfilesFriends).ThenInclude(x => x.Profile)
            //    .Where(x => x.Id == profileFriends.FriendId).AsQueryable();
            //var friend = context.Friends
            //   .Include(x => x.ProfilesFriends).ThenInclude(x => x.Profile);



            //await HttpContext.InsertParametersPaginationInHeader(queryable);
            //var friends = await queryable.OrderBy(x => x.Id).Paginate(paginationDTO).ToListAsync();
            if (profileFriends.Any())
            {
                var friends = new List<UserProfile>();
                foreach (var profileFriend in profileFriends)
                {
                    friends.Add(context.Profiles.FirstOrDefault(x => x.Id == profileFriend.FriendId));
                }

                return mapper.Map<List<UserProfile>>(friends);
            }
            return NoContent();
        }

        //[HttpPost("addFriend/{profileId}/{friendId}")]
        //public async Task<ActionResult> AddFriend([FromRoute] string profileId, [FromRoute] string friendId)
        //{
        //    var friend = await context.Profiles.FirstOrDefaultAsync(x => x.Id == friendId);
        //    var profile = await context.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
        //    var profileFriends = new ProfilesFriends { ProfileId = profileId, FriendId = friendId };
        //    var profileFriends2 = new ProfilesFriends { ProfileId = friendId, FriendId = profileId };
        //    context.Add(profileFriends);
        //    context.Add(profileFriends2);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpDelete("removeFriend/{profileId}/{friendId}")]
        public async Task<ActionResult> DeleteFriend([FromRoute] string profileId, [FromRoute] string friendId)
        {
            var friend = await context.ProfilesFriends.FirstOrDefaultAsync(x => x.FriendId == friendId && x.ProfileId == profileId);
            var friend2 = await context.ProfilesFriends.FirstOrDefaultAsync(x => x.FriendId == profileId && x.ProfileId == friendId);
            context.Remove(friend);
            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("sendFriendRequest/{userId}/{friendId}")]
        public async Task<ActionResult> SendFriendRequest([FromRoute] string userId, [FromRoute] string friendId)
        {
            var friend = await context.Profiles.FirstOrDefaultAsync(x => x.Id == friendId);
            var profile = await context.Profiles.FirstOrDefaultAsync(x => x.Id == userId);
            var friendRequest = new FriendRequest { UserId = userId, FriendId = friendId,
                SenderName = profile.Email , SenderProfileImage = profile.ProfileImage,
                FriendName = friend.Email, FriendProfileImage = friend.ProfileImage};

            context.Add(friendRequest);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("yourFriendRequests/{userId}")]    
        public async Task<ActionResult<List<FriendRequestDTO>>> YourFriendRequests([FromRoute] string userId)
        {
            var friendRequests = await context.FriendRequests.Where(x => x.FriendId == userId).ToListAsync();

            return mapper.Map<List<FriendRequestDTO>>(friendRequests);
          
        }
        [HttpGet("sentFriendRequests/{userEmail}")]
        public async Task<ActionResult<List<FriendRequestDTO>>> SentFriendRequests([FromRoute] string userEmail)
        {
            var user = await context.Profiles.FirstOrDefaultAsync(x => x.Email == userEmail);
            var friendRequests = await context.FriendRequests.Where(x => x.UserId == user.Id).ToListAsync();

            return mapper.Map<List<FriendRequestDTO>>(friendRequests);

        }

        [HttpPost("acceptFriendRequest/{profileId}/{friendId}")]
        public async Task<ActionResult> AcceptFriendRequest([FromRoute] string profileId, [FromRoute] string friendId)
        {
            var friend = await context.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
            var profile = await context.Profiles.FirstOrDefaultAsync(x => x.Id == friendId);
            var friendRequest = await context.FriendRequests.FirstOrDefaultAsync(x => x.UserId == friendId && x.FriendId == profileId);
            var profileFriends = new ProfilesFriends { ProfileId = profileId, FriendId = friendId };
            var profileFriends2 = new ProfilesFriends { ProfileId = friendId, FriendId = profileId };
            context.Remove(friendRequest);
            context.Add(profileFriends);
            context.Add(profileFriends2);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("refuseFriendRequest/{profileId}/{friendId}")]
        public async Task<ActionResult> RefuseFriendRequest([FromRoute] string profileId, [FromRoute] string friendId)
        {
            var friendRequest = await context.FriendRequests.FirstOrDefaultAsync(x => x.FriendId == profileId && x.UserId == friendId);
            context.Remove(friendRequest);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("cancelSentRequest/{profileId}/{friendId}")]
        public async Task<ActionResult> CancelSentRequest([FromRoute] string profileId, [FromRoute] string friendId)
        {
            var friendRequest = await context.FriendRequests.FirstOrDefaultAsync(x => x.FriendId == friendId && x.UserId == profileId);
            context.Remove(friendRequest);
            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("checkIfInMyFriends/{userId}")]

        public bool CheckIfInMyFriends([FromBody] object friendsInJson, [FromRoute] string userId)
        {
            var notInMyFriends = true;
            try
            {
               var friends = JsonConvert.DeserializeObject<List<ProfileDTO>>(friendsInJson.ToString());
                if (friends != null)
                {

                
                foreach (var friend in friends)
                {
                    if (friend.Id == userId)
                    {
                        notInMyFriends = false;
                    }
                }
              }
            }
            catch
            {

            }

            return notInMyFriends;

        }

        [HttpPost("checkIfInMyRequests/{senderId}")]

        public bool CheckIfInMyRequests([FromBody] object friendsInJson, [FromRoute] string senderId)
        {
            var notInMyFriends = true;
            try
            {
                var friends = JsonConvert.DeserializeObject<List<FriendRequestDTO>>(friendsInJson.ToString());
                foreach (var friend in friends)
                {
                    if (friend.FriendId == senderId)
                    {
                        notInMyFriends = false;
                    }
                }
            }
            catch
            {

            }

            return notInMyFriends;

        }
    }
}
