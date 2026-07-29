using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryMart.Models
{
    public class GroceryItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroceryItemId { get; set; }
        public GroceryItem? GroceryItem { get; set; }
        public int Quantity { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AddToCartRequest
    {
        public int GroceryItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}