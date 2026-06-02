using System;
using System.Collections.Generic; // <--- List ke liye zaroori hai
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscussionForum.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage ="The username is required.")]
    [MaxLength(100, ErrorMessage ="The username cannot exceed 100 characters.")]
    public string Username { get; set; }

    [DataType(DataType.Date)]
    public DateTime CommentDate { get; set; } = DateTime.Now; // Default current date

    [Required(ErrorMessage ="The comment content is required.")]
    public string Content { get; set; }

    // FIXED: PostId (d small rakha hai Fluent API se match karne ke liye)
    [ForeignKey("Post")]
    public int PostId { get; set; }

    // Optional parent comment for nested replies
    public int? ParentCommentId { get; set; }
    public virtual Comment ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

    // Navigation property (virtual for better EF performance)
    public virtual Post Post { get; set; }
}
