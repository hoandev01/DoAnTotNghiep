﻿using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Controllers.EmployeeArea
{
    public class EmployeesController : Controller
    {
        private readonly FarmContext _context;

        public EmployeesController(FarmContext context)
        {
            _context = context;
        }
        //get method
        public async Task<IActionResult> Index(int page = 1, string searchString = "")
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            ViewBag.CurrentFilter = searchString;
            int pageSize = 5;

            var employees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.FullName.Contains(searchString) ||
                    e.Username.Contains(searchString) ||
                    e.Email.Contains(searchString));
            }

            employees = employees.OrderBy(e => e.FullName);
            var paginatedList = await PaginatedList<Employee>.CreateAsync(employees, page, pageSize);
            return View(paginatedList);
        }


        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Username,Password,Phone,Email")] Employee employee)
        {
            // ✅ Chỉ Admin được phép tạo
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            // ✅ Kiểm tra trùng Username
            bool usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == employee.Username.ToLower());
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "❌ This username is already taken.");
            }

            // ✅ Kiểm tra trùng Email
            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == employee.Email.ToLower());
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "❌ This email is already in use.");
                }
            }

            // ✅ Kiểm tra trùng số điện thoại
            if (!string.IsNullOrWhiteSpace(employee.Phone))
            {
                bool phoneExists = await _context.Users
                    .AnyAsync(u => u.Phone == employee.Phone);
                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "❌ This phone number is already registered.");
                }
            }

            // ✅ Nếu hợp lệ → băm mật khẩu và lưu
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                employee.Password = passwordHasher.HashPassword(employee, employee.Password);

                _context.Users.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var employee = await _context.Users.OfType<Employee>().FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Username,Password,Phone,Email")] Employee employee)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingEmployee = await _context.Users.OfType<Employee>().FirstOrDefaultAsync(e => e.Id == id);
                if (existingEmployee == null) return NotFound();

                existingEmployee.FullName = employee.FullName;
                existingEmployee.Username = employee.Username;
                existingEmployee.Email = employee.Email;
                existingEmployee.Phone = employee.Phone;

                if (!string.IsNullOrEmpty(employee.Password))
                {
                    var hasher = new PasswordHasher<User>();
                    existingEmployee.Password = hasher.HashPassword(existingEmployee, employee.Password);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var employee = await _context.Users.OfType<Employee>().FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var employee = await _context.Users.OfType<Employee>().FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            var employee = await _context.Users.OfType<Employee>().FirstOrDefaultAsync(e => e.Id == id);
            if (employee != null)
            {
                _context.Users.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
