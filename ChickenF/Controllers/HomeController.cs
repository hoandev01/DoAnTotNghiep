using System.Diagnostics;
using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChickenF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FarmContext _context;

        public HomeController(ILogger<HomeController> logger, FarmContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featured = await _context.Products
                .Where(p => p.Price > 100000 && p.ProductStock > 0)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.ProductName,
                    ImageUrl = p.Image,
                    IsFeatured = p.Price > 100000 && p.ProductStock > 0,
                    IsActive = p.ProductStock > 0,
                    ShortDescription = p.ProductDescription,
                    PricePerUnit = p.Price
                })
                .Take(6)
                .ToListAsync();

            var slides = await _context.Slides
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            var news = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedDate)
                .Take(3)
                .ToListAsync();

            // ✅ Lấy 4 sản phẩm bán chạy nhất (trong các đơn hàng đã giao)
            var topSelling = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.Order.Status == "Delivered")
                .GroupBy(od => od.Product)
                .Select(group => new ProductViewModel
                {
                    Id = group.Key.Id,
                    Name = group.Key.ProductName,
                    ImageUrl = group.Key.Image,
                    IsFeatured = group.Key.Price > 100000 && group.Key.ProductStock > 0,
                    IsActive = group.Key.ProductStock > 0,
                    ShortDescription = group.Key.ProductDescription,
                    PricePerUnit = group.Key.Price,
                    TotalSold = group.Sum(x => x.OrderDetailQuantity) // nếu bạn muốn hiển thị
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(4)
                .ToListAsync();

            var model = new HomeViewModel
            {
                FeaturedProducts = featured,
                Slides = slides,
                LatestNewsArticles = news,
                TopSellingProducts = topSelling // ✅ thêm dòng này
            };

            return View(model);
        }


        [Route("Home/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View(); // bạn có thể tạo View tương ứng: Views/Home/AccessDenied.cshtml
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

        
}
