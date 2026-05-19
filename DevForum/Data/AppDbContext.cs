using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DevForum.Models;

namespace DevForum.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    //DbSet ka mtlb hai ke hum apne database me kon kon se tables create karna chahte hain. Yaha par humne Post aur Comment tables create kiye hain.
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category {Id=1,Name="Technology"},
            new Category {Id=2,Name="Health"},
            new Category {Id=3,Name="LifeStyle"}
        );

        // Configure the relationships and constraints
        modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 2,
                    Title = "Health Post 1",
                    Content = "Content of Health Post 1",
                    Author = "Jane Doe",
                    PublishedDate = new DateTime(2023, 1, 1), // Static date
                    CategoryId = 2,
                    FeatureImagePath = "health_image.jpg", // Sample image path
                },
                new Post
                {
                    Id = 3,
                    Title = "Lifestyle Post 1",
                    Content = "Content of Lifestyle Post 1",
                    Author = "Alex Smith",
                    PublishedDate = new DateTime(2023, 1, 1), // Static date
                    CategoryId = 3,
                    FeatureImagePath = "lifestyle_image.jpg", // Sample image path
                }
        );
    }
}
