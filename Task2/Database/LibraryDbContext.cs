using Microsoft.EntityFrameworkCore;
using Task2.Models.LibraryModel;

namespace Task2.Database
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            :base (options)
        { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany<Review>()
                .WithOne()
                .HasForeignKey(r => r.BookId);

            modelBuilder.Entity<Book>()
                .HasMany<Rating>()
                .WithOne()
                .HasForeignKey(r => r.BookId);
        }
    }
}
