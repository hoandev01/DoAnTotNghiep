using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChickenF.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly FarmContext _context;

        public SearchController(FarmContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string keyword)
        {
            var results = new List<SearchResultViewModel>();

            if (!string.IsNullOrEmpty(keyword))
            {
                // Sản phẩm
                results.AddRange(_context.Products
                    .Where(p => p.ProductName.Contains(keyword) || p.ProductDescription.Contains(keyword))
                    .Select(p => new SearchResultViewModel
                    {
                        Title = p.ProductName,
                        Description = p.ProductDescription,
                        Controller = "Product",
                        Action = "Details",
                        RouteValues = new { id = p.Id }
                    }));

                // Đơn hàng
                results.AddRange(_context.Orders
                    .Where(o => o.Id.ToString().Contains(keyword) || o.Status.Contains(keyword))
                    .Select(o => new SearchResultViewModel
                    {
                        Title = "Order #" + o.Id,
                        Description = "Status: " + o.Status,
                        Controller = "Order",
                        Action = "Details",
                        RouteValues = new { id = o.Id }
                    }));

                // Tin tức
                results.AddRange(_context.NewsArticles
                    .Where(n => n.Title.Contains(keyword) || n.Summary.Contains(keyword))
                    .Select(n => new SearchResultViewModel
                    {
                        Title = n.Title,
                        Description = n.Summary,
                        Controller = "News",
                        Action = "Details",
                        RouteValues = new { id = n.Id }
                    }));
            }

            return View(results);
        }
    }


}
