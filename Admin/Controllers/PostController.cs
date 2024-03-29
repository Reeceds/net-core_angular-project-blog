using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Admin;

[ApiController]
[Route("[controller]")]
public class PostController : Controller
{
    private readonly DataContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<AppUser> _userManager;

    public PostController(DataContext context, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
    {
        this._context = context;
        this._webHostEnvironment = webHostEnvironment;
        this._userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetAllPosts() // IActionResult, use when you don't need to specify what your'e returning. ActionResult is preferred when you want to clearly specify both the data type and the HTTP status code to be returned.
    {
        var posts = await this._context.Posts.Include(p => p.Images).ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(int id)
    {
        var post = await this._context.Posts.Include(x => x.Images).FirstOrDefaultAsync(p => p.Id == id);

        return Ok(post);
    }

    [Authorize]
    [HttpGet("create")]
    public IActionResult CreatePostPage()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromForm] PostVM _post)
    {
        string stringFileName = UploadFile(_post);
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string displayName = _userManager.GetUserAsync(this.User).Result.DisplayName;
        
        if (string.IsNullOrEmpty(stringFileName))
        {
            stringFileName = "jk-placeholder-image.jpeg";
        }

        var post = new Post
        {
            AppUserId = userId,
            Title = _post.Title,
            Description = _post.Description,
            CurrentImage = stringFileName,
            DisplayName = displayName
        };

        post.Images.Add(new Image
        {
            AppUserId = userId,
            ImageUrl = stringFileName,
        });

        this._context.Posts.Add(post);
        await this._context.SaveChangesAsync();
        
        Ok("Post created");
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

    [Authorize]
    [HttpGet("edit/{id}")]
    public IActionResult EditPostPage(int id)
    {
        var post = this._context.Posts.FirstOrDefault(p => p.Id == id);
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (post == null)
        {
            return BadRequest("No existing post with this Id.");
        }

        return View(new PostVM
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            AppUserId = userId,
            // CurrentImage = post.CurrentImage,
        });
    }

    [Authorize]
    [HttpPost("edit")]
    public async Task<IActionResult> EditPost([FromForm] PostVM _post)
    {
        var post = this._context.Posts.FirstOrDefault(p => p.Id == _post.Id);
        string displayName = _userManager.GetUserAsync(this.User).Result.DisplayName;

        if (post == null) return NotFound();

        _context.Entry(post).State = EntityState.Detached; // Stop tracking _context as this causes error
        
        string stringFileName = post.CurrentImage;

        if (_post.CurrentImage != null)
        {
            stringFileName = UploadFile(_post);
        }

        var updatedPost = new Post
        {
            Id = _post.Id,
            Title = _post.Title,
            Description = _post.Description,
            CurrentImage = stringFileName,
            DateCreated = DateTime.UtcNow,
            AppUserId = _post.AppUserId,
            DisplayName = displayName
        };

        if (_post.CurrentImage != null) // If a new image is posted, then update Images table with new image
        {
            updatedPost.Images.Add(new Image
            {
                ImageUrl = updatedPost.CurrentImage,
                AppUserId = _post.AppUserId,
            });
        }
        
        this._context.Posts.Update(updatedPost);
        await this._context.SaveChangesAsync();

        Ok("Post edited successfully");
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

    [Authorize]
    [HttpGet("delete/{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = this._context.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null) return BadRequest();

        var image = this._context.Images.Where(i => i.PostId == id);

        foreach (var img in image)
        {
            if (!img.ImageUrl.Contains("jk-placeholder-image.jpeg"))
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "images", img.ImageUrl));
            }
        }

        this._context.Remove(post);
        await this._context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

    public string UploadFile(PostVM vm)
    {
        string fileName = null;

        if (vm.CurrentImage != null)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            fileName = Guid.NewGuid().ToString() + "-" + vm.CurrentImage.FileName;
            string filePath = Path.Combine(uploadDir, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.CurrentImage.CopyTo(fileStream);
            }
        }

        return fileName;
    }
}
