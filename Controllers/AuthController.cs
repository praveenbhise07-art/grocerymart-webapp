using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroceryMart.Data;
using GroceryMart.Models;

namespace GroceryMart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // ... rest of your Register and Login endpoints ...
    }
}