using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Database
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        public Context() 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Name1", Email = "Email1", Password = "Password1", UserTag = "Tag1"},
                new User { Id = 2, Name = "Name2", Email = "Email2", Password = "Password2", UserTag = "Tag2" }
                );
        }
    }
}
