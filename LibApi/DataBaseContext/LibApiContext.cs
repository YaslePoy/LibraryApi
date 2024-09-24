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
    }
}
