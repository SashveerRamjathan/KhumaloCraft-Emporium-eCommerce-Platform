using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Models;

namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly KhumaloCraftDatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FeedbackController(KhumaloCraftDatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        // GET: Feedback
        public async Task<IActionResult> Index()
        {
            var khumaloCraftDatabaseContext = _context.Feedbacks.Include(f => f.User);
            return View(await khumaloCraftDatabaseContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: Feedback/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureFeedback(Feedback feedback)
        {

            //Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                feedback.UserId = currentUser.Id;

                //get the user role
                var userRoles = await _userManager.GetRolesAsync(currentUser);

                if (userRoles.Count > 0)
                {
                    feedback.UserRole = userRoles.First();
                }

                _context.Add(feedback);
                await _context.SaveChangesAsync();

                return View("FeedBackSuccessful");
            }

            return RedirectToAction("ContactUs", "Home", feedback);
        }

        [Authorize]
        public IActionResult FeedBackSuccessful()
        {
            return View();
        }

       
        [Authorize(Roles = "Admin")]
        // GET: Feedback/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        [Authorize(Roles = "Admin")]
        // POST: Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(string id)
        {
            return _context.Feedbacks.Any(e => e.UserId == id);
        }


    }
}
