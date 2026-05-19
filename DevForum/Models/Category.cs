using System;
using System.Collections.Generic; // <--- Yeh zaroori hai
using System.ComponentModel.DataAnnotations;

namespace DevForum.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="The category name is required.")]
    [MaxLength(100, ErrorMessage ="The category name cannot exceed 100 characters.")]
    public string Name { get; set; }

    public string? Description { get; set; }

    // Fix: Object ki jagah ICollection<Post> use karein
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}