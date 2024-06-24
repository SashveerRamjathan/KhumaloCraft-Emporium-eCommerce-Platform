using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Models;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{
    [Authorize(Roles = "Artist")]
    public class ArtistOrdersController : Controller
    {
        private readonly KhumaloCraftDatabaseContext _context;
        private readonly HttpClient _httpClient;

        public ArtistOrdersController(KhumaloCraftDatabaseContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            // Get the current artist's ID
            var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the orders that include the artist's products
            var orders = await _context.OrderDetails
                                       .Include(od => od.Order)
                                           .ThenInclude(o => o.User)
                                       .Include(od => od.Craftwork)
                                       .Where(od => od.Craftwork.ArtistId == artistId)
                                       .ToListAsync();

            return View(orders);
        }

		// Code Attribution
		// Method written using code from: 
		// Jignesh Trivedi
		// C# Corner
		// https://www.c-sharpcorner.com/article/calling-web-api-using-httpclient/

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderDetailId, string status)
        {
            var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderDetail = await _context.OrderDetails
                                             .Include(od => od.Order)
                                                .ThenInclude(o => o.User)
                                            .Include(od => od.Craftwork)
                                            .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId && od.Craftwork.ArtistId == artistId);

            if (orderDetail != null)
            {
                orderDetail.CraftworkStatus = status;
                await _context.SaveChangesAsync();

                var apiUrl = "https://st10361554-cldv6211-emailsenderfunction.azurewebsites.net/api/EmailSender?code=BBB1mUVhLyYKpN2FAvNAfQzx3DFR7e0mqA0yIjWc8BVzAzFu17ibdA%3D%3D";

                var user = orderDetail.Order.User;

                if (user != null)
                {
                    string userEmail = user.Email;

                    if (userEmail != null && IsValidEmail(userEmail))
                    {
                        OrderEventData eventData = new OrderEventData
                        {
                            OrderId = orderDetail.OrderId.ToString(),
                            UserEmail = userEmail,
                            Status = status,
                            NotificationDate = DateTime.Now.ToString()
                        };

                        var json = JsonConvert.SerializeObject(eventData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync(apiUrl, content);

					}
                }

				return RedirectToAction(nameof(EmailSent));

            }

            return Unauthorized();
        }

        public IActionResult EmailSent()
        { 
            	return View();
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

	public class OrderEventData
	{
		public string OrderId { get; set; }
		public string UserEmail { get; set; }
		public string Status { get; set; }
		public string NotificationDate { get; set; }
	}
}
