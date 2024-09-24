using Microsoft.EntityFrameworkCore;
using TestApi2.Model;

namespace TestApi2.DataBaseContext
{
    public class TestApiDB : DbContext
    {
        public TestApiDB(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Logins> Logins { get; set; }
    }
}
