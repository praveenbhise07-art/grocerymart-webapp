using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroceryMart.Data;   // Fixes AppDbContext reference
using GroceryMart.Models; // Fixes CartItem and GroceryItem references

namespace GroceryMart.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StoreController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/store/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var items = await _db.GroceryItems.ToListAsync();
            return Ok(items);
        }

        // GET: api/store/cart/{userId}
        [HttpGet("cart/{userId?}")]
        public async Task<IActionResult> GetCart(int? userId)
        {
            if (!userId.HasValue)
            {
                return Ok(new List<object>());
            }

            var cartItems = await _db.CartItems
                .Include(c => c.GroceryItem)
                .Where(c => c.UserId == userId.Value)
                .ToListAsync();

            return Ok(cartItems);
        }

        // POST: api/store/cart/{userId}/add
        [HttpPost("cart/{userId}/add")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] CartItemDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Invalid payload." });
            }

            int itemId = dto.GroceryItemId != 0 ? dto.GroceryItemId : dto.ProductId;

            if (itemId <= 0)
            {
                return BadRequest(new { message = "Invalid product ID provided." });
            }

            // 1. Verify the GroceryItem exists in the database
            var itemExists = await _db.GroceryItems.AnyAsync(g => g.Id == itemId);
            if (!itemExists)
            {
                return NotFound(new { message = $"Grocery item with ID {itemId} does not exist." });
            }

            // 2. Verify the User exists in the database
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound(new { message = $"User with ID {userId} does not exist in the database." });
            }

            // 3. Find or add item to cart
            var existingItem = await _db.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.GroceryItemId == itemId);

            int quantityToAdd = dto.Quantity > 0 ? dto.Quantity : 1;

            if (existingItem != null)
            {
                existingItem.Quantity += quantityToAdd;
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = userId,
                    GroceryItemId = itemId,
                    Quantity = quantityToAdd
                };
                await _db.CartItems.AddAsync(newItem);
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully." });
        }
    }

    public class CartItemDto
    {
        public int GroceryItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
    }
}