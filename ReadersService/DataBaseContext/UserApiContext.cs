using LibApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LibApi.DataBaseContext
{
    public class UserApiContext : DbContext
    {
        public UserApiContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}