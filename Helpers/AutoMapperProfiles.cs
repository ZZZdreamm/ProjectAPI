using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using Profile = AutoMapper.Profile;

namespace ProjectAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<IdentityUser, UserDTO>().ReverseMap();
            CreateMap<PostCreationDTO, Post>().ReverseMap();
            CreateMap<Post, PostCreationDTO>().ReverseMap()
               .ForMember(x => x.Id, options => options.Ignore());
            CreateMap<PostDTO, Post>();
            CreateMap<Post, PostDTO>();
            CreateMap<Profile, ProfileDTO>();
            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<UserProfile, ProfileDTO>();
            CreateMap<ProfileDTO, UserProfile>();
            CreateMap<ProfileAddImageDTO, Profile>();
            CreateMap<FriendRequest, FriendRequestDTO>();
            CreateMap<MessageCreationDTO, Message>();
            CreateMap<Message, MessageDTO>();


        }

    }
}
