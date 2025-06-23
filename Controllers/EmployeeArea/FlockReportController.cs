using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChickenF.Models.ViewModels;
using ChickenF.Models;

namespace ChickenF.Controllers.EmployeeArea
{
    public class FlockReportController : Controller
    {
        private readonly FarmContext _context;

        public FlockReportController(FarmContext context)
        {
            _context = context;
        }

        // GET: /FlockReport?startDate=2025-06-01&endDate=2025-06-30
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var flocks = await _context.Flocks
            .Include(f => f.Category)
            .Include(f => f.Cage)
            .Include(f => f.Products)
            .ThenInclude(p => p.OrderDetails)
            .ThenInclude(od => od.Order)
            .Include(f => f.Trackings)
            .ToListAsync();

            // Nếu Products hoặc Trackings không có → tạo mẫu dữ liệu tạm
            foreach (var flock in flocks)
            {
                if (flock.Products == null || !flock.Products.Any())
                {
                    // tạo sản phẩm ảo hoặc bỏ qua flock này
                    continue;
                }

                foreach (var product in flock.Products)
                {
                    if (product.OrderDetails == null)
                        product.OrderDetails = new List<OrderDetail>();
                }

                if (flock.Trackings == null)
                    flock.Trackings = new List<Tracking>();
            }

            var reports = flocks.Select(flock =>
            {
                var orders = flock.Products
                    .SelectMany(p => p.OrderDetails)
                    .Where(od => od.Order?.Status == "Delivered");

                if (startDate.HasValue)
                    orders = orders.Where(od => od.Order.OrderDate >= startDate.Value);
                if (endDate.HasValue)
                    orders = orders.Where(od => od.Order.OrderDate <= endDate.Value);

                var totalRevenue = orders.Sum(od => od.OrderDetailPrice);
                var totalSold = orders.Sum(od => od.OrderDetailQuantity);
                var feedCost = flock.Trackings.Sum(t => t.FeedCost);

                return new FlockReportViewModel
                {
                    Flock = flock,
                    TotalRaised = flock.FlockQuantity,
                    TotalSold = totalSold,
                    TotalRevenue = totalRevenue,
                    TotalFeedCost = feedCost,
                    
                    ReportGeneratedAt = DateTime.Now
                };
            }).ToList();


            return View(reports);
        }

        
    }
}
