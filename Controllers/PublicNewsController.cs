using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;

namespace ChickenF.Controllers
{
    public class PublicNewsController : Controller
    {
        private readonly FarmContext _context;

        public PublicNewsController(FarmContext context)
        {
            _context = context;
        }
        // Hiển thị danh sách tin công khai (có hỗ trợ tìm kiếm)
        public async Task<IActionResult> Index(string searchString = "")
        {
            ViewBag.CurrentFilter = searchString;

            var newsList = _context.NewsArticles.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                newsList = newsList.Where(n =>
                    n.Title.Contains(searchString) ||
                    n.Summary.Contains(searchString));
            }

            var result = await newsList
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();

            return View(result);
        }



        // Xem chi tiết một tin
        public async Task<IActionResult> Details(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }
    }
}
