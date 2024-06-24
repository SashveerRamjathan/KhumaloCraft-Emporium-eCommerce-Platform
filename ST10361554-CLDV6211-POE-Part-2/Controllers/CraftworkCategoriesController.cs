using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Models;

namespace ST10361554_CLDV6211_POE_Part_2.Controllers
{

    public class CraftworkCategoriesController : Controller
    {
        private readonly KhumaloCraftDatabaseContext _context;

        public CraftworkCategoriesController(KhumaloCraftDatabaseContext context)
        {
            _context = context;
        }
		[Authorize(Roles = "Admin, Artist")]
		// GET: CraftworkCategories
		public async Task<IActionResult> Index()
        {
            return View(await _context.CraftworkCategories.ToListAsync());
        }

		[Authorize(Roles = "Admin")]
		// GET: CraftworkCategories/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craftworkCategory = await _context.CraftworkCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (craftworkCategory == null)
            {
                return NotFound();
            }

            return View(craftworkCategory);
        }

		[Authorize(Roles = "Admin")]
		// GET: CraftworkCategories/Create
		public IActionResult Create()
        {
            return View();
        }

		// POST: CraftworkCategories/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Admin")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] CraftworkCategory craftworkCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(craftworkCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(craftworkCategory);
        }

		// GET: CraftworkCategories/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craftworkCategory = await _context.CraftworkCategories.FindAsync(id);
            if (craftworkCategory == null)
            {
                return NotFound();
            }
            return View(craftworkCategory);
        }

		// POST: CraftworkCategories/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Admin")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] CraftworkCategory craftworkCategory)
        {
            if (id != craftworkCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(craftworkCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CraftworkCategoryExists(craftworkCategory.CategoryId))
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
            return View(craftworkCategory);
        }

		// GET: CraftworkCategories/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craftworkCategory = await _context.CraftworkCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (craftworkCategory == null)
            {
                return NotFound();
            }

            return View(craftworkCategory);
        }

		// POST: CraftworkCategories/Delete/5
		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var craftworkCategory = await _context.CraftworkCategories.FindAsync(id);
            if (craftworkCategory != null)
            {
                _context.CraftworkCategories.Remove(craftworkCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CraftworkCategoryExists(int id)
        {
            return _context.CraftworkCategories.Any(e => e.CategoryId == id);
        }
    }
}
