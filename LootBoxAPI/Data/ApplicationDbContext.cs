using LootBoxAPI.Models;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Models;

namespace LootBoxAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BoxItem> BoxItems { get; set; }
    }
}
