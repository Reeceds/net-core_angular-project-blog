using Microsoft.AspNetCore.Mvc;

namespace Admin;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public string Login(string Username, string Password)
    {
        return "Username: " + Username +  " Password: " + Password;
    }
    
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Register(User model)
    {
        return View();
    }
}
