using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
            if (user == null) return NotFound("Không tìm thấy người dùng.");

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
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        [HttpGet("myorders")]
        public async Task<IActionResult> MyOrders()
        {
            int userId = GetLoggedInUserId();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new Order
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
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
                .ToListAsync();

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
        [HttpPost("cancelorder/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.UserId != GetLoggedInUserId())
                return NotFound();

            // Chỉ cho hủy nếu trạng thái là Pending
            if (order.Status == "Pending")
            {
                order.Status = "Canceled";
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
                throw new UnauthorizedAccessException("Không tìm thấy userId trong claims.");

            return int.Parse(claim.Value);
        }

    }
}
