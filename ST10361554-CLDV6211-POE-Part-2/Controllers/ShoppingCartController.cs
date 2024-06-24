using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Models;
using ST10361554_CLDV6211_POE_Part_2.Services;
using ST10361554_CLDV6211_POE_Part_2.View_Models;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;


namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{
    [Authorize(Roles = "Customer")]
	public class ShoppingCartController : Controller
	{
        private readonly KhumaloCraftDatabaseContext _context;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly HttpClient _httpClient;

        public ShoppingCartController(ShoppingCartService shoppingCartService, KhumaloCraftDatabaseContext context)
        {
            _shoppingCartService = shoppingCartService;
            _context = context;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
		{
            var cartItems = _shoppingCartService.GetCartItems();
            decimal cartTotal = CalculateCartTotal(cartItems);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cartItems,
                CartTotal = cartTotal
            };

            return View(viewModel);
        }

        // Helper method to calculate the cart total
        private decimal CalculateCartTotal(List<ShoppingCartItem> cartItems)
        {
            decimal cartTotal = 0;
            foreach (var item in cartItems)
            {
                cartTotal += item.craftwork.CraftworkPrice * item.Quantity;
            }
            return cartTotal;
        }

        public IActionResult AddToCart(int id)
        {
            var craftwork = _context.Craftworks.Find(id);

            if (craftwork == null)
            {
                return NotFound();
            }

            return View(craftwork);
        }

        [HttpPost]
        public IActionResult AddToCartConfirmed(int craftworkId, int quantity)
        {
            var craftwork = _context.Craftworks.Find(craftworkId); 
            if (craftwork == null)
            {
                return NotFound(); // Craftwork not found
            }

            var cartItem = new ShoppingCartItem
            {
                CraftworkId = craftworkId,
                Quantity = quantity,
                craftwork = craftwork
            };

            _shoppingCartService.AddToCart(cartItem);

            return RedirectToAction("Index"); // Redirect to shopping cart page
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int craftworkId)
        {
            _shoppingCartService.RemoveFromCart(craftworkId);
            return RedirectToAction("Index"); // Redirect to shopping cart page
        }

        public IActionResult ClearCart()
        {
            _shoppingCartService.ClearCart();

            return RedirectToAction("Index");
        }

		// Code Attribution
		// Method written using code from: 
		// Jignesh Trivedi
		// C# Corner
		// https://www.c-sharpcorner.com/article/calling-web-api-using-httpclient/

		public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            var shoppingCartItems = _shoppingCartService.GetCartItems();

            if (shoppingCartItems == null || shoppingCartItems.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var apiUrl = "https://st10361554-cldv6211-orderworkflowfunction.azurewebsites.net/api/OrderWorkflowFunction_HttpStart?";

            OrderRequestData requestData = new OrderRequestData
            {
                UserId = userId,
                ShoppingCartItems = shoppingCartItems
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);


            // Clear shopping cart items
            _shoppingCartService.ClearCart();

            //var orderTotalAmount = shoppingCartItems.Sum(item => item.craftwork?.CraftworkPrice * item.Quantity ?? 0);

            //// Create the Order record
            //var newOrder = new Order
            //{
            //    UserId = userId,
            //    OrderDate = DateOnly.FromDateTime(DateTime.Now),
            //    OrderTotalAmount = orderTotalAmount,
            //    OrderStatus = "Pending"
            //};
            //_context.Orders.Add(newOrder);

            //// Create OrderDetails for each item in the shopping cart
            //foreach (var item in shoppingCartItems)
            //{
            //    var orderDetail = new OrderDetail
            //    {
            //        Order = newOrder,
            //        CraftworkId = item.CraftworkId,
            //        Quantity = item.Quantity
            //    };
            //    newOrder.OrderDetails.Add(orderDetail);

            //    // Decrease Craftwork Quantity
            //    var craftwork = await _context.Craftworks.FindAsync(item.CraftworkId);

            //    if (craftwork == null)
            //    {
            //        return NotFound();
            //    }

            //    craftwork.CraftworkQuantity -= item.Quantity;
            //    _context.Craftworks.Update(craftwork);

            //}

            //// Clear shopping cart items
            //_shoppingCartService.ClearCart();

            return RedirectToAction("Index", "Orders");
        }


    }

    public class OrderRequestData
    {
        public required string UserId { get; set; }
        public required List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
