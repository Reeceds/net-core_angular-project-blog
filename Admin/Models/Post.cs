namespace Admin;

public class Post
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string CurrentImage { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public List<Image> Images { get; set; } = new List<Image>();
    public string AppUserId { get; set; }
}
