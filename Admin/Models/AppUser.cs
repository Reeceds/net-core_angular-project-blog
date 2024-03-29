using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Admin;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
}
