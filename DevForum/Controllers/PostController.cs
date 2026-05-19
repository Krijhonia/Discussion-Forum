using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevForum.Models;
using DevForum.Models.ViewModels;
using DevForum.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace DevForum.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png", ".gif" }; // Allowed image extensions
        public PostController(AppDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: PostController
        [HttpGet]
        public IActionResult Index(int? categoryId, string? searchString, int page = 1)
        {
            int pageSize = 5; // 5 threads per page
            var postQuery = _context.Posts.Include(p=>p.Category).AsQueryable();
            if(categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.CurrentCategory = categoryId;
            }
            if(!string.IsNullOrEmpty(searchString))
            {
                postQuery = postQuery.Where(p => p.Title.Contains(searchString) || p.Content.Contains(searchString));
                ViewBag.CurrentSearch = searchString;
            }

            int totalPosts = postQuery.Count();
            int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            
            // Validate page bounds
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var posts = postQuery
                .OrderByDescending(p => p.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(posts);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = _context.Posts.Include(p=>p.Category).Include(p=>p.Comments).FirstOrDefault(p => p.Id == id);
            if(post == null)
            {
                return NotFound();
            }
            return View(post);
        } 

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModel();
            postViewModel.Categories = _context.Categories.Select(c => 
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }
            ).ToList();
          
            return View(postViewModel);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("","Invalid Image Format. Allowed formats are: .jpg, .jpeg, .png, .gif");
                    return View(postViewModel);
                }

                postViewModel.Post.FeatureImagePath = await UploadFiletoFolder(postViewModel.FeatureImage);
                await _context.Posts.AddAsync(postViewModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            postViewModel.Categories = _context.Categories.Select(c => 
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }
            ).ToList();
            
            return View(postViewModel); 
    }
 
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id){
        if(id == null || id == 0)
        {
            return NotFound();
        }
        var PostFromDb = await _context.Posts.FirstOrDefaultAsync(p=>p.Id == id);
        
        if (PostFromDb == null)
        {
            return NotFound();
        }

        var editViewModel = new EditPostViewModel
        {
            Post = PostFromDb,
            Categories = await _context.Categories.Select(c => 
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }
            ).ToListAsync()
        };

        return View(editViewModel);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPostViewModel editViewModel)
    {
        if (id != editViewModel.Post.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var postFromDb = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (postFromDb == null) return NotFound();

                if (editViewModel.FeatureImage != null)
                {
                    // Optionally delete old image here
                    editViewModel.Post.FeatureImagePath = await UploadFiletoFolder(editViewModel.FeatureImage);
                }
                else
                {
                    editViewModel.Post.FeatureImagePath = postFromDb.FeatureImagePath;
                }
                
                // Retain original published date or update?
                editViewModel.Post.PublishedDate = postFromDb.PublishedDate;

                _context.Update(editViewModel.Post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(editViewModel.Post.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        editViewModel.Categories = await _context.Categories.Select(c => 
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }
        ).ToListAsync();
        return View(editViewModel);
    }

    private bool PostExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public JsonResult AddComment([FromBody]Comment comment)
    {
        comment.CommentDate =  DateTime.Now;
        _context.Comments.Add(comment);
        _context.SaveChanges();
        return Json(new{
            username = comment.Username,
            commentDate = comment.CommentDate.ToString("MMMM dd, yyyy"),
            comment =  comment.Content
        });
    }
    private async Task<string> UploadFiletoFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension; // Unique file name generate karne ke liye
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }
            var filePath = Path.Combine(imagesFolderPath, fileName);
            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch(Exception ex)
            {
                return $"Error uploading file: {ex.Message}";
            }
            return "/images/" + fileName; // Return the relative path to the uploaded image
        }   
}
}
   