using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions option) : base(option)
        {

        }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Emp> Emps { get; set; }

    }
}
