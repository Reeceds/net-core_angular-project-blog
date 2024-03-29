namespace Admin;

public class PostVM
{
    public int Id { get; set; }
    public IFormFile CurrentImage { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public List<IFormFile> Images { get; set;}
    public string AppUserId { get; set; }
}
