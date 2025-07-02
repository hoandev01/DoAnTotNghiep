using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data; // Thay thế với namespace thực tế cho DbContext
using ChickenF.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ChickenF.Controllers.EmployeeArea
{
    public class CageController : Controller
    {
        private readonly FarmContext _context; // Thay thế FarmContext với DbContext thực tế của bạn

        public CageController(FarmContext context)
        {
            _context = context;
        }

        // GET: Cage
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            var cages =  _context.Cages
                .AsNoTracking()
                .OrderByDescending(f => f.CageCapacity);
            var paginatedList = await PaginatedList<Cage>.CreateAsync(cages, page, pageSize);
            if (cages == null || !cages.Any())
            {
                return View("NoCages"); // Tạo view NoCages.cshtml nếu muốn
            }
            return View(paginatedList);
        }

        // GET: Cage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cage = await _context.Cages.FirstOrDefaultAsync(c => c.Id == id);
            if (cage == null)
                return NotFound();

            return View(cage);
        }

        // GET: Cage/Create
        public IActionResult Create()
        {
            return View(new Cage());
        }

        // POST: Cage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CageName,CageType,CageCapacity,CageArea")] Cage cage)
        {
            // Kiểm tra xem tên chuồng đã tồn tại chưa (bỏ qua chữ hoa/thường)
            bool isDuplicate = await _context.Cages
                .AnyAsync(c => c.CageName.ToLower() == cage.CageName.ToLower());

            if (isDuplicate)
            {
                ModelState.AddModelError("CageName", "❌ This cage name already exists. Please choose another name.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cage);
        }

        // GET: Cage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cage = await _context.Cages.FindAsync(id);
            if (cage == null)
                return NotFound();

            return View(cage);
        }

        // POST: Cage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CageName,CageType,CageCapacity,CageArea")] Cage cage)
        {
            if (id != cage.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CageExists(cage.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cage);
        }

        // GET: Cage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cage = await _context.Cages.FirstOrDefaultAsync(c => c.Id == id);
            if (cage == null)
                return NotFound();

            return View(cage);
        }

        // POST: Cage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cage = await _context.Cages.FindAsync(id);
            if (cage != null)
            {
                _context.Cages.Remove(cage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CageExists(int id)
        {
            return _context.Cages.Any(c => c.Id == id);
        }
    }
}
