namespace Admin;

public class Image
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int PostId { get; set; } // Foreign Key
    public Post Post { get; set; } // Reference navigation property
}
