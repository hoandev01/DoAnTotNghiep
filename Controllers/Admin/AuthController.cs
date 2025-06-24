using Microsoft.AspNetCore.Mvc;
using ChickenF.Models;
using ChickenF.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ChickenF.Controllers.EmployeeArea
{
    public class AuthController : Controller
    {
        public const string SessionKeyName = "_Name";
        private readonly FarmContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(FarmContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ================================
        // GET: Login
        // ================================
        [HttpGet]
        [Route("/login", Name = "login")]
        public IActionResult Login()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
            var viewModel = new LoginViewModel();
            return View("~/Views/Admin/Login.cshtml", viewModel);
        }

        // ================================
        // POST: Login
        // ================================
        [HttpPost]
        [Route("/login", Name = "login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == viewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Account does not exist.");
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, viewModel.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View("~/Views/Admin/Login.cshtml", viewModel);
            }

            // ✅ Xác định role từ kiểu object
            string roleString = user switch
            {
                Admin => "Admin",
                Employee => "Employee",
                Customer => "Customer",
                _ => "Customer"
            };

            // ✅ Lưu session để hiển thị tên
            HttpContext.Session.SetString(SessionKeyName, user.FullName);
            HttpContext.Session.SetString("RoleName", roleString);

            // ✅ Claims để hỗ trợ layout và authorize
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, roleString)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

            // ✅ Chuyển hướng theo role
            return roleString switch
            {
                "Admin" => RedirectToAction("Index", "Report", new { area = "Admin" }),
                "Employee" => RedirectToAction("Index", "Product", new { area = "Employee" }),
                "Customer" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Login")
            };
        }

        // ================================
        // Logout
        // ================================
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Auth");
        }

        // ================================
        // GET: Register
        // ================================
        [HttpGet]
        [Route("/register", Name = "register")]
        public IActionResult Register()
        {
            ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
            var viewModel = new RegisterViewModel();
            return View("~/Views/Admin/Register.cshtml", viewModel);
        }

        // ================================
        // POST: Register
        // ================================
        [HttpPost]
        [Route("/register", Name = "register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Name = HttpContext.Session.GetString(SessionKeyName);
                return View("~/Views/Admin/Register.cshtml", viewModel);
            }

            var existingUser = await _context.Users
                .AnyAsync(u => u.Username == viewModel.Username || u.Email == viewModel.Email);

            if (existingUser)
            {
                ModelState.AddModelError("Username", "Username or email already exists.");
                return View("~/Views/Admin/Register.cshtml", viewModel);
            }

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(new Customer(), viewModel.Password); // Default là Customer

            var newUser = new Customer
            {
                Username = viewModel.Username,
                Password = hashedPassword,
                FullName = viewModel.Name,
                Phone = viewModel.Phone,
                Email = viewModel.Email
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Auth");
        }
    }
}
