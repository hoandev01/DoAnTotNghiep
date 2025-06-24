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

        // Hiển thị danh sách tin công khai
        public async Task<IActionResult> Index()
        {
            var newsList = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();

            return View(newsList);
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
