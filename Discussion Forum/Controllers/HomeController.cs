using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscussionForum.Models;
using DiscussionForum.Models.ViewModels;
using DiscussionForum.Data;

namespace DiscussionForum.Controllers;

// HomeController: Yeh aapki website ka default controller hai.
// Jab koi user sirf base URL (jaise www.mysite.com) par aata hai, toh yeh controller chalta hai.
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // Index Action: Yeh website ka "Home Page" hai.
    // Jab user "/" ya "/Home/Index" URL par jayega, toh yeh method execute hoga.
    public IActionResult Index()
    {
        var recentPosts = _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.PublishedDate)
            .Take(5)
            .ToList();

        var trendingPosts = _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.Comments.Count)
            .ThenByDescending(p => p.PublishedDate)
            .Take(5)
            .ToList();

        var categoryStats = _context.Categories
            .Include(c => c.Posts)
                .ThenInclude(p => p.Comments)
            .Select(c => new CategoryStat
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PostCount = c.Posts.Count,
                CommentCount = c.Posts.SelectMany(p => p.Comments).Count(),
                LatestPost = c.Posts.OrderByDescending(p => p.PublishedDate).FirstOrDefault()
            })
            .ToList();

        var viewModel = new HomeViewModel
        {
            RecentPosts = recentPosts,
            TrendingPosts = trendingPosts,
            CategoryStats = categoryStats,
            TotalPosts = _context.Posts.Count(),
            TotalComments = _context.Comments.Count(),
            TotalMembers = _context.Users.Count()
        };

        return View(viewModel);
    }

    // Privacy Action: Yeh Privacy Policy page ke liye hai.
    // Iska URL hoga: "/Home/Privacy"
    public IActionResult Privacy()
    {
        // Yeh "Views/Home/Privacy.cshtml" file ko user ko dikhayega.
        return View();
    }

    // Error Handling: Agar app mein koi error aata hai, toh yeh action trigger hota hai.
    // [ResponseCache] attribute: Yeh browser ko bol raha hai ki error page ko cache (save) mat karo, 
    // kyunki error hamesha naya aur updated hona chahiye.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Yeh ek 'ErrorViewModel' banata hai jisme 'RequestId' hoti hai.
        // RequestId se developer ko pata chalta hai ki server par kaunsa specific request fail hua tha.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
