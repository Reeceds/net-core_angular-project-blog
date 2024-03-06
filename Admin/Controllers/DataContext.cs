using Microsoft.EntityFrameworkCore;

namespace Admin;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Image> Images { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    // protected override void OnModelCreating(ModelBuilder builder) // Built in method, must spell conrrect 'OnModelCreating'
    // {
    //     base.OnModelCreating(builder);

    //     builder.Entity<Post>()
    //         .HasMany(el => el.Images)
    //         .WithOne(el => el.Post)
    //         .HasForeignKey(el => el.PostId)
    //         .IsRequired();
    // }
}
