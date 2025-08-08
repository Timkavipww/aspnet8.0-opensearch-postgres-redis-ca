using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithMany(b => b.Authors)
            .UsingEntity(j => j.ToTable("AuthorBooks"));


        modelBuilder.Entity<User>()
            .HasData(new User
            {
                Id = 1,
                Name = "Admin",
                Email = "123"
            },
            new User
            {
                Id = 2,
                Name = "User",
                Email = "asd"
            },
            new User
            {
                Id = 3,
                Name = "User",
                Email = "qwe"
            }
            );
               
    }
}
