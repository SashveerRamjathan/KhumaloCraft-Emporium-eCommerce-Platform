using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Services;
using System.Security.Claims;

namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
	{
		private readonly KhumaloCraftDatabaseContext _context;

        public OrdersController(KhumaloCraftDatabaseContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {

            // Get the UserId of the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch orders for the current user
            var orders = await _context.Orders
                .Include(o => o.OrderDetails) 
                    .ThenInclude(od => od.Craftwork) 
                .Where(o => o.UserId == userId) // Filter by UserId
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            // Get the UserId of the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the order details for the specified orderId and UserId
            var order = await _context.Orders
                .Include(o => o.OrderDetails) 
                    .ThenInclude(od => od.Craftwork) 
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound(); // Order not found or not belonging to the user
            }

            // Check item statuses
            bool allPending = order.OrderDetails.All(od => od.CraftworkStatus == "Pending");
            bool anyInProgress = order.OrderDetails.Any(od => od.CraftworkStatus == "In Progress");
            bool allShipped = order.OrderDetails.All(od => od.CraftworkStatus == "Shipped");

            // Update order status based on item statuses
            if (allPending)
            {
                order.OrderStatus = "Pending";
            }
            else if (anyInProgress)
            {
                order.OrderStatus = "In Progress";
            }
            else if (allShipped)
            {
                order.OrderStatus = "Shipped";
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return View(order);
        }





    }
}
