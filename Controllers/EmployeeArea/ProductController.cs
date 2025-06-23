using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data; // Thay thế với namespace thực tế cho DbContext
using ChickenF.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ChickenF.Controllers.EmployeeArea
{
    public class ProductController : Controller
    {
        private readonly FarmContext _context;

        public ProductController(FarmContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 3;

            var products =  _context.Products
                .Include(p => p.Flock)
                .AsNoTracking()
                .OrderByDescending(f => f.DateCreated);
            var paginatedList = await PaginatedList<Product>.CreateAsync(products, page, pageSize);
            if (products == null || !products.Any())
            {
                return View("NoData"); // Tạo view NoData.cshtml để thông báo không có dữ liệu
            }
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName");
            ViewData["FeedType"] = new SelectList(_context.Flocks, "Id", "FeedType");
            ViewData["ChickenSize"] = new SelectList(_context.Flocks, "Id", "ChickenSize");
            return View(paginatedList);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Flock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["FeedType"] = new SelectList(_context.Flocks, "Id", "FeedType");
            ViewData["ChickenSize"] = new SelectList(_context.Flocks, "Id", "ChickenSize");
            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName");
            ViewData["FeedType"] = new SelectList(_context.Flocks, "Id", "FeedType");
            ViewData["ChickenSize"] = new SelectList(_context.Flocks, "Id", "ChickenSize");
            return View(new Product());
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlockId,ProductName,Image,Price,ProductStock,DateCreated,ProductDescription")] Product product)
        {
            if (!ModelState.IsValid)
            {
                // Ghi log lỗi nếu có
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Model error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName");
                return View(product);
            }

            // Kiểm tra FlockId có tồn tại trong DB
            var flock = await _context.Flocks.FirstOrDefaultAsync(f => f.Id == product.FlockId);
            if (flock == null)
            {
                ModelState.AddModelError("FlockId", "Flock không tồn tại.");
                ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", product.FlockId);
                return View(product);
            }

            Console.WriteLine($"FlockId: {product.FlockId}");

            // Không cần gán Flock điều hướng (EF chỉ cần FlockId đúng là đủ)
            // Nhưng nếu bạn muốn điều hướng Flock trong object, có thể giữ dòng sau:
            product.Flock = flock;

            // Gán ngày tạo nếu cần
            product.DateCreated = DateTime.Now;

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", product.FlockId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlockId,ProductName,Image,Price,ProductStock,DateCreated,ProductDescription")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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

            ViewData["FlockId"] = new SelectList(_context.Flocks, "Id", "FlockName", product.FlockId);
            return View(product);
        }
        // GET: /Product/SyncStockForFlock/5
        public async Task<IActionResult> SyncStockForFlock(int flockId)
        {
            var flock = await _context.Flocks
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == flockId);

            if (flock == null)
            {
                TempData["Error"] = "Can not found Flocks.";
                return RedirectToAction("Index");
            }

            if (flock.Products != null && flock.Products.Any())
            {
                foreach (var product in flock.Products)
                {
                    product.ProductStock = flock.FlockQuantity;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Inventory synchronized by flock.";
            }
            else
            {
                TempData["Warning"] = "This flock has no products yet.";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetFlockInfo(int flockId)
        {
            var flock = await _context.Flocks.FindAsync(flockId);
            if (flock == null)
                return NotFound();

            return Json(new
            {
                flockName = flock.FlockName,
                flockQuantity = flock.FlockQuantity
            });
        }

        //sync all stock
        [HttpPost]
        public async Task<IActionResult> SyncAllStock()
        {
            var flocks = await _context.Flocks
                .Include(f => f.Products)
                .ToListAsync();

            foreach (var flock in flocks)
            {
                if (flock.Products != null && flock.Products.Any())
                {
                    foreach (var product in flock.Products)
                    {
                        product.ProductStock = flock.FlockQuantity;
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "All product stocks have been synchronized with their corresponding flocks.";
            return RedirectToAction("Index");
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Flock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

