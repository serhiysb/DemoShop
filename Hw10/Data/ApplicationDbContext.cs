using Hw10.Models;
using Microsoft.EntityFrameworkCore;

namespace Hw10.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
