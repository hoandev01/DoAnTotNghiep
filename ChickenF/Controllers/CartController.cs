using ChickenF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;
using System.Security.Claims;

namespace ChickenF.Controllers
{
    public class CartController : Controller
    {
        private readonly FarmContext _context;

        public CartController(FarmContext context)
        {
            _context = context;
        }

        [Route("/cart", Name = "cart.index")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Please log in to view the cart.";
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Unable to identify the user. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }

            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, redirectToLogin = true, message = "You need to log in to add items to your cart." });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, redirectToLogin = true, message = "Unable to identify the user. Please log in again." });
            }

            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "The product does not exist." });
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            int currentCartQty = cartItem?.CartItemQuantity ?? 0;
            int totalAfterAdd = currentCartQty + quantity;

            if (totalAfterAdd > product.ProductStock)
            {
                return Json(new
                {
                    success = false,
                    message = $"Không thể thêm {quantity} sản phẩm. Hiện bạn đã có {currentCartQty}, tồn kho chỉ còn {product.ProductStock}."
                });
            }

            if (cartItem != null)
            {
                cartItem.CartItemQuantity = totalAfterAdd;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    CartItemQuantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng." });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, redirectToLogin = true, message = "You need to log in to update the cart." });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, redirectToLogin = true, message = "Unable to identify the user. Please log in again." });
            }

            var cartItem = _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefault(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item does not exist in the cart." });
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else if (quantity > cartItem.Product.ProductStock)
            {
                return Json(new
                {
                    success = false,
                    message = $"Số lượng vượt quá tồn kho. Tồn kho hiện tại: {cartItem.Product.ProductStock}."
                });
            }
            else
            {
                cartItem.CartItemQuantity = quantity;
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Cart updated successfully." });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var cartItem = _context.CartItems.FirstOrDefault(c => c.Id == cartItemId && c.UserId == userId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
                TempData["CartMessage"] = "🗑️ Product removed from your cart.";
            }

            return RedirectToAction("Index");
        }


        private int? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }
    }
}
