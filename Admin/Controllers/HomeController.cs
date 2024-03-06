using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers;

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
        PostViewModel myModel = new PostViewModel();
        myModel.Posts = _context.Posts.ToList();
        return View(myModel);
    }
}
