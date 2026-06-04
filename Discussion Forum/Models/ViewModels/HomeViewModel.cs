namespace DiscussionForum.Models.ViewModels;

public class HomeViewModel
{
    public List<Post> RecentPosts { get; set; } = new();
    public List<Post> TrendingPosts { get; set; } = new();
    public List<CategoryStat> CategoryStats { get; set; } = new();
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalMembers { get; set; }
}

public class CategoryStat
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
    public Post? LatestPost { get; set; }
}
