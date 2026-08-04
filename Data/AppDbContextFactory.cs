using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GroceryMart.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            // Default to SQLite for local development migrations:
            optionsBuilder.UseSqlite("Data Source=grocerymart.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}