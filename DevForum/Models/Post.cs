using System;
using System.Collections.Generic; // <--- Is line ko add karna zaroori hai List/ICollection ke liye
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DevForum.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "The title is required.")]
    [MaxLength(400, ErrorMessage = "The title cannot exceed 400 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "The content is required.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "The author is required.")]
    [MaxLength(100, ErrorMessage = "The author name cannot exceed 100 characters.")]
    public string Author { get; set; }

    [ValidateNever]
    public string? FeatureImagePath { get; set; } // Nullable rakha hai agar image na ho

    [DataType(DataType.Date)]
    public DateTime PublishedDate { get; set; }

    // FIXED: CategoryId (d small rakhein taaki AppDbContext ki Fluent API se match kare)
    [ForeignKey("Category")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    // Navigation property (virtual lagane se Lazy Loading mein madad milti hai)
    [ValidateNever]
    public virtual Category Category { get; set; }

    // FIXED: ICollection ko hamesha new List se initialize karein
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public int ViewCount { get; set; } = 0;
    public int Upvotes { get; set; } = 0;
} 