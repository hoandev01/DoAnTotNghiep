using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ChickenF.Helpers;

namespace ChickenF.Controllers.UserArea
{
    [Authorize]
    [Route("user")]
    public class AccountController : Controller
    {
        private readonly FarmContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(FarmContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("profile", Name = "user.profile")]
        public IActionResult Profile()
        {
            int userId = GetLoggedInUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("Can not found user.");

            return View("~/Views/User/Profile.cshtml", user);
        }

        [HttpGet("editprofile")]
        public IActionResult EditProfile()
        {
            int userId = GetLoggedInUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new EditUserViewModel { FullName = user.FullName };
            return View("~/Views/User/EditProfile.cshtml", model);
        }

        [HttpPost("editprofile")]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/User/EditProfile.cshtml", model);

            int userId = GetLoggedInUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            if (!string.IsNullOrWhiteSpace(model.Password))
                user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Update information successfully!";
            return RedirectToAction("Profile");
        }
        [HttpGet("myorders")]
        public async Task<IActionResult> MyOrders(string search, string status, DateTime? fromDate, DateTime? toDate)
        {
            int userId = GetLoggedInUserId();

            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .AsQueryable();

            // Tìm theo OrderId hoặc từ khóa
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    query = query.Where(o => o.Id == id);
                else
                    query = query.Where(o => o.CancelReason.Contains(search));
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }



            // Lọc theo ngày
            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();

            ViewData["Search"] = search;
            ViewData["Status"] = status;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            return View("~/Views/User/MyOrders.cshtml", orders);
        }


        [HttpGet("orderdetails/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new Order
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CancelReason = o.CancelReason, // 👈 THÊM DÒNG NÀY
                    OrderDetails = o.OrderDetails
                        .Where(od => od.Product != null)
                        .Select(od => new OrderDetail
                        {
                            Id = od.Id,
                            OrderId = od.OrderId,
                            ProductId = od.ProductId,
                            OrderDetailQuantity = od.OrderDetailQuantity,
                            OrderDetailPrice = od.OrderDetailPrice,
                            Product = od.Product
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null || order.UserId != GetLoggedInUserId())
                return NotFound();

            return View("~/Views/User/OrderDetails.cshtml", order);
        }

        [HttpPost("cancelorder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId, string reason)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.UserId != GetLoggedInUserId())
                return NotFound();

            if (order.Status == "Pending")
            {
                order.Status = "Cancelled";
                order.CancelReason = reason;

                // 👇 Cập nhật ReservedQuantity
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.ReservedQuantity -= detail.OrderDetailQuantity;

                        // Đảm bảo không âm
                        if (detail.Product.ReservedQuantity < 0)
                            detail.Product.ReservedQuantity = 0;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Order has been canceled successfully.";
            }
            else
            {
                TempData["Error"] = "Only pending orders can be canceled.";
            }

            return RedirectToAction(nameof(MyOrders));
        }



        private int GetLoggedInUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("Can not found userid in claim.");

            return int.Parse(claim.Value);
        }

    }
}
