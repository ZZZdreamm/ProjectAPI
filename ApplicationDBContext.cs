using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace ProjectAPI
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext([NotNullAttribute] DbContextOptions options) : base(options)
        {

        }

        
        public DbSet<Post> Posts { get; set; }
        public DbSet<Profile> Profiles { get;set; }

    }
}
