using GroceryMart.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryMart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HealthController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                int userCount = await _db.Users.AsNoTracking().CountAsync();
                int itemCount = await _db.GroceryItems.AsNoTracking().CountAsync();

                return Ok(new
                {
                    status = "Healthy",
                    database = "Connected (SQLite)",
                    userCount,
                    itemCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { status = "Unhealthy", error = ex.Message });
            }
        }
    }
}