using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Models;

namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{
    public class WorkController : Controller
	{
		private readonly KhumaloCraftDatabaseContext _context;

		public WorkController(KhumaloCraftDatabaseContext context) 
		{
			_context = context;
		}

		// GET: Work/Index
		public async Task<IActionResult> Index()
		{
			ViewBag.Categories = _context.CraftworkCategories.ToList();
			
			var craftworks = await _context.Craftworks
			.Include(c => c.Artist)
			.Include(c => c.Category)
			.ToListAsync();

			return View(craftworks);
		}

		// GET: Work/ViewArtistDetails/5
		public async Task<IActionResult> ArtistDetails(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var craftwork = await _context.Craftworks
				.Include(c => c.Artist)
				.FirstOrDefaultAsync(m => m.CraftworkId == id);

			if (craftwork == null)
			{
				return NotFound();
			}

			var artist = craftwork.Artist;
			if (artist == null)
			{
				return NotFound();
			}

			return View(artist);
		}

		public IActionResult FilterByCategory(int? categoryId)
		{
			ViewBag.Categories = _context.CraftworkCategories.ToList();
			List<CraftworkCategory> categories = _context.CraftworkCategories.ToList();

			if (categoryId.HasValue)
			{
				foreach (var category in categories)
				{
					if (category.CategoryId == categoryId)
					{
						ViewBag.SelectedCategoryName = category.CategoryName;
					}
				}
			}

			var craftworks = _context.Craftworks.AsQueryable();

			if (categoryId.HasValue && categoryId > 0)
			{
				craftworks = craftworks.Where(c => c.CategoryId == categoryId);
			}

			return View("Index", craftworks.ToList());
		}
	}
}
