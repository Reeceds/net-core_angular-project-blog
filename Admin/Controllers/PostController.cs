using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Admin;

[ApiController]
[Route("[controller]")]
public class PostController : Controller
{
    private readonly DataContext _context;
    private readonly IWebHostEnvironment webHostEnvironment;

    public PostController(DataContext context, IWebHostEnvironment webHostEnvironment)
    {
        this._context = context;
        this.webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult GetAllPosts()
    {
        var posts = this._context.Posts.ToList();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    public IActionResult GetPost(int id)
    {
        // var post = this._context.Posts.Include(x => x.Images).FirstOrDefault(p => p.Id == id);
        var post = this._context.Posts.FirstOrDefault(p => p.Id == id);

        return Ok(post);
    }

    [HttpGet("create")]
    public IActionResult CreatePostPage()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromForm] SinglePostViewModel _post)
    {
        string stringFileName = UploadFile(_post);
        
        if (string.IsNullOrEmpty(stringFileName))
        {
            stringFileName = "jk-placeholder-image.jpeg";
        }

        var post = new Post
        {
            Title = _post.Title,
            Description = _post.Description,
            CurrentImage = stringFileName,
        };

        post.Images.Add(new Image
        {
            ImageUrl = stringFileName,
        });

        this._context.Posts.Add(post);
        await this._context.SaveChangesAsync();
        
        Ok("Post created");
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

    [HttpGet("edit/{id}")]
    public IActionResult EditPostPage(int id)
    {
        var post = this._context.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null)
        {
            return BadRequest("No existing post with this Id.");
        }

        return View(new SinglePostViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            // CurrentImage = post.CurrentImage,
        });
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditPost([FromForm] SinglePostViewModel _post)
    {
        var post = this._context.Posts.FirstOrDefault(p => p.Id == _post.Id);

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
        };

        if (_post.CurrentImage != null) // If a new image is posted, then update Images table with new image
        {
            updatedPost.Images.Add(new Image
            {
                ImageUrl = updatedPost.CurrentImage,
            });
        }
        
        this._context.Posts.Update(updatedPost);
        await this._context.SaveChangesAsync();

        Ok("Post edited successfully");
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

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
                System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, "images", img.ImageUrl));
            }
        }

        this._context.Remove(post);
        await this._context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Home"); // Folder 'Home', file 'Index'
    }

    public string UploadFile(SinglePostViewModel vm)
    {
        string fileName = null;

        if (vm.CurrentImage != null)
        {
            string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
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
