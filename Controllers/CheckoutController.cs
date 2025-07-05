using ChickenF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using ChickenF.Helpers;

namespace ChickenF.Controllers
{
    public class CheckoutRequest
    {
        public string PaymentMethod { get; set; }
    }

    public class CheckoutController : Controller
    {
        private readonly FarmContext _context;

        public CheckoutController(FarmContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/checkout", Name = "checkout.index")]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToLoginWithError("Login first.");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Empty cart.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }

        [HttpPost]
        [Route("/checkout/process", Name = "checkout.process")]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return JsonError("Login first.");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return JsonError("empty cart.");

            switch (request.PaymentMethod?.ToLower())
            {
                case "cash":
                    return await HandleCashPayment(cartItems, userId.Value);

                case "bank":
                case "momo":
                    string qrImage = request.PaymentMethod == "bank" ? "/Image/qr_nganhang.jpg" : "/Image/qr_momo.jpg";
                    return Json(new { success = true, showQR = true, qrImageUrl = qrImage });

                default:
                    return JsonError("Error.");
            }
        }

        [HttpGet]
        [Route("/success", Name = "checkout.success")]
        public async Task<IActionResult> Success()
        {
            if (!TempData.ContainsKey("OrderId"))
                return RedirectToAction("Index", "Home");

            int orderId = (int)TempData["OrderId"];

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return RedirectToAction("Index", "Home");

            return View(order);
        }

        // ----------- PRIVATE HELPERS ----------------------

        private async Task<IActionResult> HandleCashPayment(List<CartItem> cartItems, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var productMap = new Dictionary<int, Product>();

                foreach (var item in cartItems)
                {
                    var product = await _context.Products
                        .Include(p => p.Flock)
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null)
                        return JsonError($"❌ Product does not exist (ID: {item.ProductId}).");

                    int availableStock = product.ProductStock - product.ReservedQuantity;
                    if (item.CartItemQuantity > availableStock)
                        return JsonError($"❌ Not enough stock for {product.ProductName}. Available: {availableStock}");

                    // ✅ Giữ hàng
                    product.ReservedQuantity += item.CartItemQuantity;

                    // Nếu hết hàng thật → cập nhật OutOfStock
                    if (availableStock == item.CartItemQuantity && product.OutOfStockAt == null)
                        product.OutOfStockAt = DateTime.Now;

                    await UpdateFlockStatusIfCompleted(product.FlockId);

                    productMap[item.ProductId] = product;
                }

                // Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    PaymentMethod = "Cash on Delivery",
                    Status = OrderStatus.Pending.ToString(),
                    TotalAmount = cartItems.Sum(item =>
                        item.CartItemQuantity * productMap[item.ProductId].Price),
                    OrderDetails = cartItems.Select(item => new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderDetailPrice = productMap[item.ProductId].Price,
                        OrderDetailQuantity = item.CartItemQuantity
                    }).ToList()
                };

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["OrderId"] = order.Id;
                return Json(new { success = true, redirectUrl = Url.Action("Success", "Checkout") });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return JsonError("❌ Failed to process order: " + ex.Message);
            }
        }


        private async Task UpdateFlockStatusIfCompleted(int flockId)
        {
            var flock = await _context.Flocks
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == flockId);

            if (flock == null) return;

            bool allOutOfStock = flock.Products.All(p => p.ProductStock <= 0);

            if (allOutOfStock && flock.Status != "Sold out")
            {
                flock.Status = "Sold out";

                // Ghi thêm giai đoạn “Đã xuất bán” vào bảng FlockStage
                var finalStage = new FlockStage
                {
                    FlockId = flock.Id,
                    StageName = "Sold out.",
                    StartDate = DateTime.Now,
                    Note = "Automatically update after all products have been sold out."
                };
                _context.FlockStages.Add(finalStage);

                

                await _context.SaveChangesAsync();
            }
        }


        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userId, out int id) ? id : (int?)null;
        }

        private IActionResult RedirectToLoginWithError(string message)
        {
            TempData["Error"] = message;
            return RedirectToAction("Login", "Auth");
        }

        private JsonResult JsonError(string message)
        {
            return Json(new { success = false, message });
        }
    }
}
