using ChickenF.Data;
using ChickenF.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChickenF.Controllers.EmployeeArea
{
    public class ReportController : Controller
    {
        private readonly FarmContext _context;

        public ReportController(FarmContext context)
        {
            _context = context;
        }

        // Trang chủ báo cáo
        public async Task<IActionResult> Index()
        {
            var flocks = await _context.Flocks
                .Include(f => f.Category)
                .Include(f => f.Cage)
                .Include(f => f.Products)
                    .ThenInclude(p => p.OrderDetails)
                        .ThenInclude(od => od.Order)
                .Include(f => f.Trackings)
                .ToListAsync();

            var reports = flocks.Select(flock =>
            {
                var orders = flock.Products
                    .SelectMany(p => p.OrderDetails)
                    .Where(od => od.Order?.Status == "Delivered");

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

            return View(reports); // ✅ Trả về model đúng
        }

        // GET: /Admin/Report/GetTopSellingProducts
        [HttpGet]
        public IActionResult GetTopSellingProducts()
        {
            var topProducts = _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                .Select(g => new
                {
                    name = g.Key.ProductName,
                    quantity = g.Sum(x => x.OrderDetailQuantity)
                })
                .OrderByDescending(x => x.quantity)
                .Take(5)
                .ToList();

            return Json(topProducts);
        }



        // GET: /Admin/Report/GetOrderStatusDistribution
        [HttpGet]
        public async Task<IActionResult> GetOrderStatusDistribution()
        {
            var data = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new {
                    status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            return Json(data);
        }

        // GET: /Admin/Report/GetMonthlyRevenue
        [HttpGet]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var data = await Task.Run(() =>
                _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .AsEnumerable()
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new
                    {
                        month = $"{g.Key.Month}/{g.Key.Year}",
                        totalRevenue = g.Sum(x => (decimal)x.TotalAmount)
                    })
                    .OrderBy(x => x.month)
                    .ToList() // ✅ Chạy đúng với LINQ to Objects
            );

            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetFlockReportData()
        {
            var flocks = await _context.Flocks
                .Include(f => f.Category)
                .Include(f => f.Cage)
                .Include(f => f.Products)
                    .ThenInclude(p => p.OrderDetails)
                        .ThenInclude(od => od.Order)
                .Include(f => f.Trackings)
                .ToListAsync();

            var reports = flocks.Select(flock =>
            {
                var orders = flock.Products
                    .SelectMany(p => p.OrderDetails)
                    .Where(od => od.Order?.Status == "Delivered");

                var totalRevenue = orders.Sum(od => od.OrderDetailPrice);
                var totalSold = orders.Sum(od => od.OrderDetailQuantity);
                var feedCost = flock.Trackings.Sum(t => t.FeedCost);

                return new
                {
                    flock = new
                    {
                        flock.FlockName,
                        category = flock.Category != null ? new { flock.Category.CategoryName } : null,
                        cage = flock.Cage != null ? new { flock.Cage.CageName } : null
                    },
                    totalRaised = flock.FlockQuantity,
                    totalSold,
                    totalRevenue,
                    totalFeedCost = feedCost,
                    profit = totalRevenue - feedCost,
                    reportGeneratedAt = DateTime.Now
                };
            }).ToList();

            return Json(reports);
        }


    }
}
