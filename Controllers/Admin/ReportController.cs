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

                // ✅ Đã sửa: nhân số lượng với đơn giá để tính đúng doanh thu
                var totalRevenue = orders.Sum(od => od.OrderDetailPrice * od.OrderDetailQuantity);

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

        [HttpGet]
        public async Task<IActionResult> GetDashboardKPIs()
        {
            // ✅ Revenue = Quantity × Price
            var totalRevenue = await _context.OrderDetails
                .Where(od => od.Order.Status == "Delivered")
                .SumAsync(od => (decimal?)od.OrderDetailQuantity * od.Product.Price) ?? 0;

            // ✅ Lấy các Flock đã bán → tránh cộng phí của flock chưa có đơn hàng
            var soldFlockIds = await _context.OrderDetails
                .Select(od => od.Product.FlockId)
                .Distinct()
                .ToListAsync();

            var totalFeedCost = await _context.Trackings
                .Where(t => soldFlockIds.Contains(t.FlockId))
                .SumAsync(t => (decimal?)t.FeedCost) ?? 0;

            var totalProfit = totalRevenue - totalFeedCost;

            var totalOrders = await _context.Orders.CountAsync();

            var activeFlocks = await _context.Flocks.CountAsync(f => f.Status == "Still Raising");

            return Json(new
            {
                totalRevenue,
                totalProfit,
                totalOrders,
                activeFlocks
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetRevenueByRange(DateTime from, DateTime to)
        {
            var data = await _context.Orders
                .Where(o => o.Status == "Delivered" && o.OrderDate >= from && o.OrderDate <= to)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new {
                    date = g.Key.ToString("dd/MM/yyyy"),
                    revenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemWarnings()
        {
            var highPsdFlocks = await _context.Flocks
                .Include(f => f.Cage)
                .Where(f => (double)f.FlockQuantity / f.Cage.CageArea > 15) // tùy logic PSD
                .Select(f => f.FlockName)
                .ToListAsync();

            var lowStockProducts = await _context.Products
                .Where(p => p.ProductStock < 10)
                .Select(p => new { p.ProductName, p.ProductStock })
                .ToListAsync();

            var oldPendingOrders = await _context.Orders
                .Where(o => o.Status == "Pending" && o.OrderDate < DateTime.Now.AddDays(-5))
                .Select(o => new { o.Id, o.OrderDate })
                .ToListAsync();

            return Json(new
            {
                highPsdFlocks,
                lowStockProducts,
                oldPendingOrders
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyGrowth()
        {
            var revenues = await _context.Orders
                .Where(o => o.Status == "Delivered")
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.year).ThenBy(x => x.month)
                .ToListAsync();

            var growth = new List<object>();
            for (int i = 1; i < revenues.Count; i++)
            {
                var previous = revenues[i - 1];
                var current = revenues[i];
                double rate = (double)(current.revenue - previous.revenue) / (previous.revenue == 0 ? 1 : previous.revenue);
                growth.Add(new
                {
                    month = $"{current.month}/{current.year}",
                    rate = Math.Round(rate * 100, 2)
                });
            }

            return Json(growth);
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryData()
        {
            var inventory = await _context.Products
                .Select(p => new {
                    name = p.ProductName,
                    stock = p.ProductStock
                })
                .ToListAsync();

            return Json(inventory);
        }




    }
}
