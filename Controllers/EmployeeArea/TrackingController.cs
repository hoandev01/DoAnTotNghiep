using System.Threading;
using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Controllers.EmployeeArea
{
    public class TrackingController : Controller
    {
        private readonly FarmContext _context;

        public TrackingController(FarmContext context)
        {
            _context = context;
        }

        // GET: FlockTracking
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var tracking = _context.Trackings
                .Include(f => f.Flock)
                .AsNoTracking()
                .OrderByDescending(f => f.TrackingDate);
            var paginatedList = await PaginatedList<Tracking>.CreateAsync(tracking, page, pageSize);
            return View(paginatedList);
        }

        // GET: FlockTracking/Create
        public IActionResult Create()
        {
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName");
            return View();
        }

        // POST: FlockTracking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tracking tracking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tracking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", tracking.FlockId);
            return View(tracking);
        }

        // GET: FlockTracking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracking = await _context.Trackings.FindAsync(id);
            if (tracking == null)
            {
                return NotFound();
            }
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", tracking.FlockId);
            return View(tracking);
        }

        // POST: FlockTracking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tracking tracking)
        {
            if (id != tracking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tracking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackingExists(tracking.Id))
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
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", tracking.FlockId);
            return View(tracking);
        }

        // GET: FlockTracking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracking = await _context.Trackings
                .Include(f => f.Flock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracking == null)
            {
                return NotFound();
            }

            return View(tracking);
        }

        // GET: FlockTracking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracking = await _context.Trackings
                .Include(f => f.Flock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracking == null)
            {
                return NotFound();
            }

            return View(tracking);
        }

        // POST: FlockTracking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tracking = await _context.Trackings.FindAsync(id);
            if (tracking != null)
            {
                _context.Trackings.Remove(tracking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrackingExists(int id)
        {
            return _context.Trackings.Any(e => e.Id == id);
        }
    }
}
