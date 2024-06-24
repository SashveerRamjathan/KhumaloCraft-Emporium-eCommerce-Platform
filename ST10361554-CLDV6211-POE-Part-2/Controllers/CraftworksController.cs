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
    [Authorize(Roles = "Artist, Admin")]
    public class CraftworksController : Controller
    {
        private readonly KhumaloCraftDatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CraftworksController(KhumaloCraftDatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Craftworks
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string confirmedArtistID = "-1";


            List<Artist> artists = _context.Artists.ToList();

            foreach (Artist artist in artists)
            {
                if (artist.ArtistId == userId)
                {
                    confirmedArtistID = userId;
                }
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            IQueryable<Craftwork> craftworksQuery;

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                craftworksQuery = _context.Craftworks
                                     .Include(c => c.Artist)
                                     .Include(c => c.Category);
            }
            else
            {
                craftworksQuery = _context.Craftworks
                .Include(c => c.Artist)
                .Include(c => c.Category)
                .Where(c => c.ArtistId == confirmedArtistID);
            }

            
           
            return View(await craftworksQuery.ToListAsync());
        }

        // GET: Craftworks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craftwork = await _context.Craftworks
                .Include(c => c.Artist)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.CraftworkId == id);
            if (craftwork == null)
            {
                return NotFound();
            }

            return View(craftwork);
        }

        // GET: Craftworks/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string confirmedArtistID = "-1";


            List<Artist> artists = _context.Artists.ToList();

            foreach (Artist artist in artists)
            {
                if (artist.ArtistId == userId)
                {
                    confirmedArtistID = userId;
                }
            }

            ViewData["CurrentUserId"] = confirmedArtistID;

            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "ArtistId");
            ViewData["CategoryId"] = new SelectList(_context.CraftworkCategories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Craftworks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CraftworkId,CraftworkName,CraftworkDescription,CraftworkPrice,CraftworkQuantity,CategoryId,ArtistId,CraftworkPictureData,ImageFile")] Craftwork craftwork)
        {
            byte[] bytes;

            if (craftwork.ImageFile != null)
            {
                using (Stream fs = craftwork.ImageFile.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }

                craftwork.CraftworkPictureData = bytes;
            }

            

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string confirmedArtistID = "-1";


            List<Artist> artists = _context.Artists.ToList();

            foreach (Artist artist in artists)
            {
                if (artist.ArtistId == userId)
                {
                    confirmedArtistID = userId;
                }
            }

            if (!ModelState.IsValid && !confirmedArtistID.Equals("-1"))
            {
                _context.Add(craftwork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = confirmedArtistID;
            ViewData["CategoryId"] = new SelectList(_context.CraftworkCategories, "CategoryId", "CategoryId", craftwork.CategoryId);
            return View(craftwork);
        }

        // GET: Craftworks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string confirmedArtistID = "-1";


            List<Artist> artists = _context.Artists.ToList();

            foreach (Artist artist in artists)
            {
                if (artist.ArtistId == userId)
                {
                    confirmedArtistID = userId;
                }
            }

            ViewData["CurrentUserId"] = confirmedArtistID;

            var craftwork = await _context.Craftworks.FindAsync(id);
            if (craftwork == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "ArtistId", craftwork.ArtistId);
            ViewData["CategoryId"] = new SelectList(_context.CraftworkCategories, "CategoryId", "CategoryId", craftwork.CategoryId);
            return View(craftwork);
        }

        // POST: Craftworks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CraftworkId,CraftworkName,CraftworkDescription,CraftworkPrice,CraftworkQuantity,CategoryId,ArtistId,CraftworkPictureData,ImageFile")] Craftwork craftwork)
        {
            if (id != craftwork.CraftworkId)
            {
                return NotFound();
            }

			
			byte[] bytes;

            if (craftwork.ImageFile != null)
            {
                using (Stream fs = craftwork.ImageFile.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }

                craftwork.CraftworkPictureData = bytes;
            }
            else
            {
                var currentProduct = await _context.Craftworks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CraftworkId == id);

                if (currentProduct != null)
                {
                    craftwork.CraftworkPictureData = currentProduct.CraftworkPictureData;
                }
            }


            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(craftwork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CraftworkExists(craftwork.CraftworkId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "ArtistId", craftwork.ArtistId);
            ViewData["CategoryId"] = new SelectList(_context.CraftworkCategories, "CategoryId", "CategoryId", craftwork.CategoryId);
            return View(craftwork);
        }

        // GET: Craftworks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craftwork = await _context.Craftworks
                .Include(c => c.Artist)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.CraftworkId == id);
            if (craftwork == null)
            {
                return NotFound();
            }

            return View(craftwork);
        }

        // POST: Craftworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var craftwork = await _context.Craftworks.FindAsync(id);
            if (craftwork != null)
            {
                _context.Craftworks.Remove(craftwork);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CraftworkExists(int id)
        {
            return _context.Craftworks.Any(e => e.CraftworkId == id);
        }
    }
}
