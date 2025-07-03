using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data; 
using ChickenF.Models;
using System.Threading.Tasks;

namespace ChickenF.Controllers.EmployeeArea
{
    public class FlockController : Controller
    {
        private readonly FarmContext _context; 

        public FlockController(FarmContext context)
        {
            _context = context;
        }

        // GET: Flock
        public async Task<IActionResult> Index(DateTime? selectedDate, string sortOrder)
        {
            DateTime today = selectedDate ?? DateTime.Today;
            ViewBag.SelectedDate = today;

            // Dùng ViewBag để hỗ trợ đổi chiều sắp xếp
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var flocks = await _context.Flocks
                .Include(f => f.Category)
                .Include(f => f.Cage)
                .Include(f => f.FlockStages)
                .ToListAsync();

            // Logic stage như cũ
            var viewModelList = flocks.Select(flock => {
                var stages = flock.FlockStages.OrderBy(fs => fs.StartDate).ToList();
                string currentStage = "Unknown";
                string status = "";
                DateTime? suggestedSaleDate = null;

                var matchedStage = stages.FirstOrDefault(s => s.StartDate <= today && today <= s.EndDate);
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
                    if (lastStage != null && today > lastStage.EndDate)
                    {
                        currentStage = "Ready for Sale";
                        status = "✅ Ready for market";
                        suggestedSaleDate = lastStage.EndDate?.AddDays(1);
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

            // ✳️ Áp dụng sắp xếp
            viewModelList = sortOrder switch
            {
                "name_desc" => viewModelList.OrderByDescending(v => v.Flock.FlockName).ToList(),
                "Date" => viewModelList.OrderBy(v => v.Flock.DayIn).ToList(),
                "date_desc" => viewModelList.OrderByDescending(v => v.Flock.DayIn).ToList(),
                _ => viewModelList.OrderBy(v => v.Flock.FlockName).ToList(),
            };

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
            ViewBag.AllCages = _context.Cages.ToList();
            ViewData["CageId"] = new SelectList(_context.Cages, "Id", "CageName");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        //post create flock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlockName,CageId,CategoryId,FlockQuantity,ChickenSize,FeedType,Status,GrowthLevel,FlockNote,DayIn")] Flock flock)
        {
            if (flock.DayIn == DateTime.MinValue || flock.DayIn == default)
            {
                flock.DayIn = DateTime.Today;
            }

            // ✅ Kiểm tra trùng tên Flock (không phân biệt chữ hoa/thường)
            bool nameExists = await _context.Flocks
                .AnyAsync(f => f.FlockName.ToLower() == flock.FlockName.ToLower());

            if (nameExists)
            {
                ModelState.AddModelError("FlockName", "❌ This flock name already exists.");
            }

            // ✅ Lấy thông tin chuồng để tính PSD & Capacity
            var cage = await _context.Cages.FirstOrDefaultAsync(c => c.Id == flock.CageId);
            if (cage == null)
            {
                ModelState.AddModelError("CageId", "Selected cage does not exist.");
            }
            else
            {
                // ➤ Tính mật độ nuôi
                double psd = (double)flock.FlockQuantity / cage.CageArea;

                // ➤ Lấy giới hạn PSD theo loại chuồng
                double maxPsd = cage.CageType switch
                {
                    "Closed" => 15.0,
                    "Open" => 12.0,
                    "Semi-open" => 13.0,
                    "Elevated" => 16.0,
                    _ => 15.0
                };

                if (psd > maxPsd)
                {
                    ModelState.AddModelError("", $"❌ Overstocked: PSD = {psd:F2} birds/m² in a '{cage.CageType}' cage. Max allowed is {maxPsd}.");
                }

                // ➤ Kiểm tra vượt Capacity
                int currentQuantityInCage = await _context.Flocks
                    .Where(f => f.CageId == cage.Id)
                    .SumAsync(f => f.FlockQuantity);

                int projectedTotal = currentQuantityInCage + flock.FlockQuantity;

                if (projectedTotal > cage.CageCapacity)
                {
                    ModelState.AddModelError("", $"❌ Capacity exceeded: After adding, total birds = {projectedTotal}, but '{cage.CageName}' allows max {cage.CageCapacity}.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Flocks.Add(flock);
                await _context.SaveChangesAsync();

                // ✅ Lấy category để tính các giai đoạn
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

                TempData["Success"] = "✅ Flock created successfully with stages.";
                return RedirectToAction("Index");
            }

            // ❗️Nếu có lỗi → load lại dropdown
            ViewBag.AllCages = await _context.Cages.ToListAsync();
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
                    // Cập nhật Flock
                    _context.Update(flock);
                    await _context.SaveChangesAsync();

                    // ✅ Tạo lại FlockStages nếu có thay đổi DayIn hoặc CategoryId
                    var category = await _context.Categories.FindAsync(flock.CategoryId);
                    if (category != null)
                    {
                        // Xoá các giai đoạn cũ
                        var oldStages = await _context.FlockStages
                            .Where(fs => fs.FlockId == flock.Id)
                            .ToListAsync();

                        _context.FlockStages.RemoveRange(oldStages);

                        // Tính lại giai đoạn mới
                        var broodingStart = flock.DayIn;
                        var broodingEnd = broodingStart.AddDays(category.BroodingDays - 1);
                        var growthEnd = broodingEnd.AddDays(category.GrowthDays);
                        var preSaleEnd = growthEnd.AddDays(category.PreSaleDays);
                        var readySaleEnd = preSaleEnd.AddDays(category.ReadyDays);

                        var newStages = new List<FlockStage>
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
                        Note = "Ready for market"
                    }
                };

                        _context.FlockStages.AddRange(newStages);
                        await _context.SaveChangesAsync();
                    }

                    // ✅ Đồng bộ các sản phẩm liên quan
                    var products = await _context.Products
                        .Where(p => p.FlockId == flock.Id)
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        product.ProductStock = flock.FlockQuantity;
                        product.ProductName = flock.FlockName;
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Flock updated, stages recalculated, and product stocks synchronized.";
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

            // Lấy sản phẩm liên quan
            var products = await _context.Products
                .Where(p => p.FlockId == id)
                .ToListAsync();

            var productIds = products.Select(p => p.Id).ToList();

            // x Nếu có sản phẩm đã giao → không cho xoá
            var hasDeliveredOrders = await _context.OrderDetails
                .Where(od => productIds.Contains(od.ProductId) && od.Order.Status == "Delivered")
                .AnyAsync();

            if (hasDeliveredOrders)
            {
                TempData["Error"] = "⚠ Cannot delete this flock because some related products have been delivered to customers.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            // Xoá các bản ghi liên quan
            var flockStages = await _context.FlockStages.Where(fs => fs.FlockId == id).ToListAsync();
            var trackings = await _context.Trackings.Where(t => t.FlockId == id).ToListAsync();
            var cartItems = await _context.CartItems.Where(ci => productIds.Contains(ci.ProductId)).ToListAsync();
            var orderDetails = await _context.OrderDetails.Where(od => productIds.Contains(od.ProductId)).ToListAsync();
            var affectedOrderIds = orderDetails.Select(od => od.OrderId).Distinct().ToList();

            foreach (var orderId in affectedOrderIds)
            {
                var stillHasOtherDetails = await _context.OrderDetails
                    .AnyAsync(od => od.OrderId == orderId && !productIds.Contains(od.ProductId));

                if (!stillHasOtherDetails)
                {
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                        _context.Orders.Remove(order);
                }
            }

            _context.FlockStages.RemoveRange(flockStages);
            _context.Trackings.RemoveRange(trackings);
            _context.CartItems.RemoveRange(cartItems);
            _context.OrderDetails.RemoveRange(orderDetails);
            _context.Products.RemoveRange(products);
            _context.Flocks.Remove(flock);

            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Flock and related data deleted successfully.";
            return RedirectToAction(nameof(Index));
        }




    }
}
