using System.ComponentModel.DataAnnotations;

namespace Admin;

public class LoginVM
{
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }

    [Required(ErrorMessage =" Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
