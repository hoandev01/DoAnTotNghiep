using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Controllers.EmployeeArea
{
    public class FlockStageController : Controller
    {
        private readonly FarmContext _context;

        public FlockStageController(FarmContext context)
        {
            _context = context;
        }

        // GET: /FlockStage
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var stages = await _context.FlockStages
                .Include(fs => fs.Flock)
                .OrderBy(fs => fs.FlockId)
                .ThenBy(fs => fs.StartDate)
                .ToListAsync();

            // Đánh dấu stage hiện tại
            foreach (var stage in stages)
            {
                if (stage.StartDate <= today && today <= stage.EndDate)
                {
                    stage.Note += " [Current Stage]";
                }
            }

            return View(stages);
        }


        // GET: FlockStage/Create?flockId=12
        public IActionResult Create(int flockId)
        {
            var stage = new FlockStage
            {
                FlockId = flockId
            };

            ViewBag.FlockList = new SelectList(_context.Flocks, "Id", "FlockName", flockId);
            return View(stage);
        }

        // POST: FlockStage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlockId,StageName,StartDate,EndDate,Note")] FlockStage stage)
        {
            if (ModelState.IsValid)
            {
                _context.FlockStages.Add(stage);
                await _context.SaveChangesAsync();

                TempData["Success"] = "FlockStage created successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.FlockList = new SelectList(_context.Flocks, "Id", "FlockName", stage.FlockId);
            return View(stage);
        }

        // GET: /FlockStages/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var stage = await _context.FlockStages
                .Include(fs => fs.Flock) // Include Flock để dùng FlockName nếu cần
                .FirstOrDefaultAsync(fs => fs.Id == id);

            if (stage == null) return NotFound();

            ViewBag.FlockList = new SelectList(_context.Flocks, "Id", "FlockName", stage.FlockId);
            return View(stage);
        }

        // POST: /FlockStages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlockId,StageName,StartDate,EndDate,Note")] FlockStage stage)
        {
            if (id != stage.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stage);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Stage updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FlockStages.Any(e => e.Id == stage.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.FlockList = new SelectList(_context.Flocks, "Id", "FlockName", stage.FlockId);
            return View(stage);
        }

        // GET: /FlockStages/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var stage = await _context.FlockStages
                .Include(fs => fs.Flock)
                .FirstOrDefaultAsync(fs => fs.Id == id);

            if (stage == null) return NotFound();

            return View(stage);
        }

        // POST: /FlockStages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stage = await _context.FlockStages.FindAsync(id);
            if (stage != null)
            {
                _context.FlockStages.Remove(stage);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "FlockStage deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
