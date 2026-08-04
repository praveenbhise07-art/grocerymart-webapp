using Microsoft.EntityFrameworkCore;
using GroceryMart.Models; // Fixes missing User, GroceryItem, and CartItem references

namespace GroceryMart.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<GroceryItem> GroceryItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}