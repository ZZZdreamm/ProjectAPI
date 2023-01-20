using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Security.Claims;

namespace ProjectAPI
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext([NotNullAttribute] DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            //builder.Entity<ProfilesFriends>()
            //   .HasKey(x => new { x.FriendId, x.ProfileId });
            base.OnModelCreating(builder);
        }




        public DbSet<Post> Posts { get; set; }
        public DbSet<UserProfile> Profiles { get;set; }
        public DbSet<ProfilesFriends> ProfilesFriends { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

    }
}
