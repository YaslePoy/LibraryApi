using LibApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LibApi.DataBaseContext
{
    public class LibApiContext : DbContext
    {
        public LibApiContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<BookRental> BookRentals { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BooksGenre> BooksGenres { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
    }
}