using Microsoft.AspNetCore.Mvc;
using ChickenF.Models;
using ChickenF.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace ChickenF.Controllers.EmployeeArea
{
    [Authorize(Roles = "Admin")]
    [Route("admin/user")]
    public class AdminUserController : Controller
    {
        public const string SessionKeyName = "_Name";
        private readonly FarmContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AdminUserController(FarmContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("/admin/users")]
        public IActionResult List()
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(sessionName)) return Redirect("/login");

            ViewBag.nameSession = sessionName;
            var userList = _context.Users.ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_UserList", userList);

            return View("~/Views/Admin/User/Index.cshtml", userList);
        }

        [HttpGet("/admin/user/add")]
        public IActionResult Add()
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(sessionName)) return Redirect("/login");

            ViewBag.nameSession = sessionName;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AddUser", new Customer());


            return View("~/Views/Admin/User/Add.cshtml", new Customer());

        }

        [HttpPost("/admin/user/insert")]
        [ValidateAntiForgeryToken]
        public IActionResult Insert([Bind("Username, Name, Password, Role")] User user)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(sessionName)) return Redirect("/login");

            ViewBag.nameSession = sessionName;

            if (ModelState.IsValid)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true, message = "User added successfully!" });

                return RedirectToAction("List");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AddUser", user);

            return View("~/Views/Admin/User/Add.cshtml", user);
        }

        [HttpPost("/admin/user/search")]
        public JsonResult Search(string search)
        {
            var sessionName = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrEmpty(sessionName))
                return Json(new { success = false, message = "Please log in!" });

            var lst = _context.Users
                .FromSqlRaw("SELECT * FROM Users WHERE Username LIKE @search OR Name LIKE @search",
                    new SqlParameter("@search", $"%{search}%"))
                .ToList();

            string[] arr_role = { "", "Quản trị viên", "Nhân viên" };

            if (lst.Any())
                return Json(new { success = true, arr_user = lst, role = arr_role });

            return Json(new { success = false, message = "No results found!" });
        }

        // ===== ADMIN PROFILE =====
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            int userId = GetLoggedInUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("Không tìm thấy người dùng.");

            return View("~/Views/Admin/Profile.cshtml", user);
        }

        [HttpGet("editprofile")]
        public IActionResult EditProfile()
        {
            int userId = GetLoggedInUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new EditUserViewModel { FullName = user.FullName };
            return View("~/Views/Admin/EditProfile.cshtml", model);
        }

        [HttpPost("editprofile")]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/EditProfile.cshtml", model);

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

        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ===== HELPER METHOD =====
        private int GetLoggedInUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("Không tìm thấy userId trong claims.");

            return int.Parse(claim.Value);
        }
    }
}
