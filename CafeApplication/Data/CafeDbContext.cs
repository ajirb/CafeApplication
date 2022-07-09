using CafeApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApplication.Data
{
    public class CafeDbContext : DbContext
    {
        public CafeDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<MenuItem> Items { get; set; }
    }
}
