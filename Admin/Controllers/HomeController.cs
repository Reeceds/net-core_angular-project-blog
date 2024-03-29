using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
        this._context = context;
    }

    public IActionResult Index()
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        PostListVM myModel = new PostListVM();

        myModel.Posts = _context.Posts.Where(p => p.AppUserId == userId).ToList();

        return View(myModel);
    }
}
