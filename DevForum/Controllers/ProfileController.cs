using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevForum.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DevForum.Models;

namespace DevForum.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;

            var myPosts = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Where(p => p.Author == username)
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            var myComments = await _context.Comments
                .Include(c => c.Post)
                .Where(c => c.Username == username)
                .OrderByDescending(c => c.CommentDate)
                .ToListAsync();

            ViewBag.StartedThreadsCount = myPosts.Count;
            ViewBag.RepliesCount = myComments.Count;

            var viewModel = new UserProfileViewModel
            {
                MyPosts = myPosts,
                MyComments = myComments
            };

            return View(viewModel);
        }
    }

    public class UserProfileViewModel
    {
        public List<Post> MyPosts { get; set; } = new List<Post>();
        public List<Comment> MyComments { get; set; } = new List<Comment>();
    }
}
