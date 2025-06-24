using ChickenF.Data;
using ChickenF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Controllers.EmployeeArea
{
    public class CustomersController : Controller
    {
        private readonly FarmContext _context;

        public CustomersController(FarmContext context)
        {
            _context = context;
        }

        // ================================
        // GET: List Customers
        // ================================
        public async Task<IActionResult> Index(int page = 1)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            int pageSize = 5;
            var customers = _context.Customers.OrderBy(c => c.FullName);
            var paginatedList = await PaginatedList<Customer>.CreateAsync(customers, page, pageSize);
            return View(paginatedList);
        }


        // ================================
        // GET: Create
        // ================================
        public IActionResult Create()
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            return View();
        }

        // ================================
        // POST: Create
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Username,Password,Phone,Email")] Customer customer)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                customer.Password = passwordHasher.HashPassword(customer, customer.Password);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // ================================
        // GET: Edit
        // ================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // ================================
        // POST: Edit
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Username,Password,Phone,Email")] Customer updatedCustomer)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id != updatedCustomer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Customers.FindAsync(id);
                if (existing == null) return NotFound();

                existing.FullName = updatedCustomer.FullName;
                existing.Username = updatedCustomer.Username;
                existing.Phone = updatedCustomer.Phone;
                existing.Email = updatedCustomer.Email;

                if (!string.IsNullOrWhiteSpace(updatedCustomer.Password))
                {
                    var passwordHasher = new PasswordHasher<User>();
                    existing.Password = passwordHasher.HashPassword(existing, updatedCustomer.Password);
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedCustomer);
        }

        // ================================
        // GET: Details
        // ================================
        public async Task<IActionResult> Details(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // ================================
        // GET: Delete
        // ================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // ================================
        // POST: Delete
        // ================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin"))
                return View("~/Views/Shared/Unauthorized.cshtml");

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
