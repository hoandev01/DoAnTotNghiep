using ChickenF.Data;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            int pageSize = 5;
            var employees = _context.Employees.OrderBy(c => c.FullName);
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
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

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
