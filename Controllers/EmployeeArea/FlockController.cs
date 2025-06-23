using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data; // Thay thế với namespace thực tế cho DbContext
using ChickenF.Models;
using System.Threading.Tasks;

namespace ChickenF.Controllers.EmployeeArea
{
    public class FlockController : Controller
    {
        private readonly FarmContext _context; // Thay thế FarmContext với DbContext thực tế của bạn

        public FlockController(FarmContext context)
        {
            _context = context;
        }

        // GET: Flock
        public async Task<IActionResult> Index(DateTime? selectedDate)
        {
            DateTime today = selectedDate ?? DateTime.Today;
            ViewBag.SelectedDate = today;

            var flocks = await _context.Flocks
                .Include(f => f.Category)
                .Include(f => f.Cage)
                .Include(f => f.FlockStages) // <--- thêm luôn include stages
                .ToListAsync();

            var viewModelList = flocks.Select(flock =>
            {
                var stages = flock.FlockStages
                    .OrderBy(fs => fs.StartDate)
                    .ToList();

                string currentStage = "Unknown";
                string status = "";
                DateTime? suggestedSaleDate = null;

                var matchedStage = stages.FirstOrDefault(s =>
                    s.StartDate <= today && today <= s.EndDate);

                if (matchedStage != null)
                {
                    currentStage = matchedStage.StageName;

                    if (matchedStage.EndDate.HasValue)
                    {
                        int daysLeft = (matchedStage.EndDate.Value - today).Days;

                        if (daysLeft > 0)
                            status = $"⏳ {currentStage} - {daysLeft} day(s) remaining";
                        else if (daysLeft == 0)
                            status = $"📌 {currentStage} ends today";
                        else
                            status = $"⚠️ {currentStage} ended {Math.Abs(daysLeft)} day(s) ago";
                    }
                    else
                    {
                        status = $"🔄 {currentStage} - no end date defined";
                    }
                }
                else
                {
                    var lastStage = stages.OrderByDescending(s => s.EndDate).FirstOrDefault();
                    if (lastStage != null)
                    {
                        if (today > lastStage.EndDate)
                        {
                            currentStage = "Ready for Sale";
                            status = "✅ Ready for market";
                            suggestedSaleDate = lastStage.EndDate?.AddDays(1);
                        }
                        else
                        {
                            status = "🕒 Stage not started yet";
                        }
                    }
                    else
                    {
                        status = "📄 No stages defined";
                    }
                }

                return new FlockViewModel
                {
                    Flock = flock,
                    CurrentStageName = currentStage,
                    StatusMessage = status,
                    SuggestedSaleDate = suggestedSaleDate
                };
            }).ToList();

            return View(viewModelList);
        }





        // GET: Flock/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var flock = await _context.Flocks
                .Include(f => f.Cage)
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flock == null) return NotFound();

            return View(flock);
        }



        // GET: Flock/Create
        public IActionResult Create()
        {
            ViewData["CageId"] = new SelectList(_context.Cages, "Id", "CageName");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlockName,CageId,CategoryId,FlockQuantity,ChickenSize,FeedType,Status,GrowthLevel,FlockNote,DayIn")] Flock flock)
        {
            if (flock.DayIn == DateTime.MinValue || flock.DayIn == default)
            {
                flock.DayIn = DateTime.Today;
            }

            if (ModelState.IsValid)
            {
                // Lưu flock trước để lấy Id
                _context.Flocks.Add(flock);
                await _context.SaveChangesAsync();

                // Lấy thông tin thời gian từ Category
                var category = await _context.Categories.FindAsync(flock.CategoryId);
                if (category == null)
                {
                    TempData["Error"] = "Invalid category.";
                    return RedirectToAction("Index");
                }

                var broodingStart = flock.DayIn;
                var broodingEnd = broodingStart.AddDays(category.BroodingDays - 1);
                var growthEnd = broodingEnd.AddDays(category.GrowthDays);
                var preSaleEnd = growthEnd.AddDays(category.PreSaleDays);
                var readySaleEnd = preSaleEnd.AddDays(category.ReadyDays);

                var stages = new List<FlockStage>
        {
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Brooding Stage",
                StartDate = broodingStart,
                EndDate = broodingEnd,
                Note = "Chicks just hatched"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Growth Stage",
                StartDate = broodingEnd.AddDays(1),
                EndDate = growthEnd,
                Note = "Rapid growth phase"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Pre-Sale Stage",
                StartDate = growthEnd.AddDays(1),
                EndDate = preSaleEnd,
                Note = "Getting ready for sale"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Ready for Sale",
                StartDate = preSaleEnd.AddDays(1),
                EndDate = readySaleEnd,
                Note = "Can be sold now"
            }
        };

                _context.FlockStages.AddRange(stages);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Flock created successfully with stages.";
                return RedirectToAction("Index");
            }

            ViewData["CageId"] = new SelectList(_context.Cages, "Id", "CageName", flock.CageId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", flock.CategoryId);
            return View(flock);
        }


        [HttpPost]
        [Route("Flocks/GenerateStages")]
        public async Task<IActionResult> GenerateStages()
        {
            var flocks = await _context.Flocks
                .Where(f => !_context.FlockStages.Any(fs => fs.FlockId == f.Id))
                .ToListAsync();

            int createdCount = 0;

            foreach (var flock in flocks)
            {
                var category = await _context.Categories.FindAsync(flock.CategoryId);
                if (category == null) continue;

                var broodingStart = flock.DayIn;
                var broodingEnd = broodingStart.AddDays(category.BroodingDays - 1);
                var growthEnd = broodingEnd.AddDays(category.GrowthDays);
                var preSaleEnd = growthEnd.AddDays(category.PreSaleDays);
                var readySaleEnd = preSaleEnd.AddDays(category.ReadyDays);

                var stages = new List<FlockStage>
        {
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Brooding Stage",
                StartDate = broodingStart,
                EndDate = broodingEnd,
                Note = "Chicks just hatched"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Growth Stage",
                StartDate = broodingEnd.AddDays(1),
                EndDate = growthEnd,
                Note = "Rapid growth phase"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Pre-Sale Stage",
                StartDate = growthEnd.AddDays(1),
                EndDate = preSaleEnd,
                Note = "Getting ready for sale"
            },
            new FlockStage
            {
                FlockId = flock.Id,
                StageName = "Ready for Sale",
                StartDate = preSaleEnd.AddDays(1),
                EndDate = readySaleEnd,
                Note = "Can be sold now"
            }
        };

                _context.FlockStages.AddRange(stages);
                createdCount++;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"✅ Stages have been created for   {createdCount} đflocks of chickens that do not have stages yet." });
        }



        // GET: Flock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flock = await _context.Flocks.FindAsync(id);
            if (flock == null)
            {
                return NotFound();
            }
            ViewData["CageId"] = new SelectList(_context.Cages, "Id", "CageName", flock.CageId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", flock.CategoryId);
            return View(flock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlockName,DayIn,CageId,CategoryId,FlockQuantity,ChickenSize,FeedType,Status,GrowthLevel,FlockNote")] Flock flock)
        {
            if (id != flock.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flock);
                    await _context.SaveChangesAsync();

                    // Đồng bộ stock các sản phẩm liên quan
                    var products = await _context.Products
                        .Where(p => p.FlockId == flock.Id)
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        product.ProductStock = flock.FlockQuantity;
                        product.ProductName = flock.FlockName;
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Flock updated and product stocks synchronized.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlockExists(flock.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CageId"] = new SelectList(_context.Cages, "Id", "CageName", flock.CageId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", flock.CategoryId);
            return View(flock);
        }



        private bool FlockExists(int id)
        {
            return _context.Flocks.Any(f => f.Id == id);
        }




        // GET: Flock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flock = await _context.Flocks
                .Include(f => f.Cage) // Nếu cần thông tin về Cage
                .Include(f => f.Category) // Nếu cần thông tin về Category
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flock == null)
            {
                return NotFound();
            }

            return View(flock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flock = await _context.Flocks.FindAsync(id);
            if (flock == null)
                return NotFound();

            // 1. Xóa FlockStages
            var flockStages = await _context.FlockStages
                .Where(fs => fs.FlockId == id)
                .ToListAsync();
            _context.FlockStages.RemoveRange(flockStages);

            

            // 3. Xóa Trackings
            var trackings = await _context.Trackings
                .Where(t => t.FlockId == id)
                .ToListAsync();
            _context.Trackings.RemoveRange(trackings);

            // 4. Lấy tất cả sản phẩm thuộc flock
            var products = await _context.Products
                .Where(p => p.FlockId == id)
                .ToListAsync();

            var productIds = products.Select(p => p.Id).ToList();

            
            

            // 🔹 Thêm: Xóa CartItems chứa các product này (nếu dùng giỏ hàng)
            var cartItems = await _context.CartItems
                .Where(ci => productIds.Contains(ci.ProductId))
                .ToListAsync();
            _context.CartItems.RemoveRange(cartItems);

            // 5. Lấy tất cả OrderDetail liên quan tới sản phẩm đó
            var orderDetails = await _context.OrderDetails
                .Where(od => productIds.Contains(od.ProductId))
                .ToListAsync();

            // 6. Ghi nhận danh sách OrderId bị ảnh hưởng
            var affectedOrderIds = orderDetails.Select(od => od.OrderId).Distinct().ToList();

            // 7. Xóa Orders không còn OrderDetail nào (sau khi trừ đi những cái sắp xóa)
            foreach (var orderId in affectedOrderIds)
            {
                var stillHasDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == orderId && !orderDetails.Contains(od))
                    .AnyAsync();

                if (!stillHasDetails)
                {
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                        _context.Orders.Remove(order);
                }
            }

            // 8. Xóa OrderDetails
            _context.OrderDetails.RemoveRange(orderDetails);

            // 9. Xóa Products
            _context.Products.RemoveRange(products);

            // 10. Xóa Flock
            _context.Flocks.Remove(flock);

            // 11. Lưu thay đổi
            await _context.SaveChangesAsync();

            TempData["Success"] = "Flock and all related data deleted successfully.";
            return RedirectToAction(nameof(Index));
        }




    }
}
