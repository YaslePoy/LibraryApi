using Microsoft.EntityFrameworkCore;
using TestApi2.Model;

namespace TestApi2.DataBaseContext
{
    public class LibApiContext : DbContext
    {
        public LibApiContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
