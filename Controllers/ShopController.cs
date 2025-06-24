using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Models;
using ChickenF.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ChickenF.Controllers
{
    public class ShopController : Controller
    {
        private readonly FarmContext _context;

        public ShopController(FarmContext context)
        {
            _context = context;
        }

        [Route("/shop", Name = "shop.index")]
        public async Task<IActionResult> Index(string search, int? categoryId, string sort)
        {
            var query = _context.Products
                .Include(p => p.Flock)
                    .ThenInclude(f => f.Category)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // Lọc theo loại
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.Flock.CategoryId == categoryId.Value);
            }

            // Sắp xếp
            query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.ProductName),
            };

            var products = await query.ToListAsync();

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "CategoryName", categoryId);
            ViewData["Search"] = search;
            ViewData["Sort"] = sort;

            return View(products);
        }



        // GET: Product/Detail/5
        [Route("/shop/{id}", Name = "shop.detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product); // Trả về 1 sản phẩm, tương ứng view Detail.cshtml @model Product
        }


    }
}