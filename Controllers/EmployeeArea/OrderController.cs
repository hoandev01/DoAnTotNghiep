using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;
using ChickenF.Models;
using ChickenF.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;

namespace ChickenF.Controllers.EmployeeArea
{
    public class OrdersController : Controller
    {
        private readonly FarmContext _context;
        private readonly IServiceProvider _serviceProvider;

        public OrdersController(FarmContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> Index(string search, string status, int page = 1)
        {
            await CleanupCancelledOrders();

            int pageSize = 5;
            var query = _context.Orders
                .Include(o => o.User)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.UserId.ToString().Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var paginatedList = await PaginatedList<Order>.CreateAsync(query, page, pageSize);

            ViewData["Search"] = search;
            ViewData["Status"] = status;

            return View(paginatedList);
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Accept Order (Hangfire Version)
        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status != OrderStatus.Pending)
                return NotFound();
            order.Status = OrderStatus.Shipping;
            await _context.SaveChangesAsync();
            // Đặt Job Hangfire sau 1 phút
            BackgroundJob.Schedule(() => UpdateOrderToDelivered(id), TimeSpan.FromMinutes(1));
            return RedirectToAction("Index");
        }
        public async Task UpdateOrderToDelivered(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null && order.Status == OrderStatus.Shipping)
            {
                order.Status = OrderStatus.Delivered;
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.Status != OrderStatus.Pending)
                return NotFound();

            // ✅ Trả lại hàng
            foreach (var detail in order.OrderDetails)
            {
                var product = detail.Product;
                product.ReservedQuantity -= detail.OrderDetailQuantity;
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelReason = reason;
            order.CancelledAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Cancelled;
            order.CancelReason = reason;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.Status != OrderStatus.Shipping)
                return NotFound();

            foreach (var detail in order.OrderDetails)
            {
                var product = detail.Product;
                product.ProductStock -= detail.OrderDetailQuantity;
                product.ReservedQuantity -= detail.OrderDetailQuantity;

                if (product.ProductStock <= 0 && product.OutOfStockAt == null)
                    product.OutOfStockAt = DateTime.Now;
            }

            order.Status = OrderStatus.Delivered;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,OrderDate,TotalAmount,PaymentMethod,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,OrderDate,TotalAmount,PaymentMethod,Status")] Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null) _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CleanupCancelledOrders()
        {
            var thresholdDate = DateTime.Now.AddDays(-10);
            var oldCancelledOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Cancelled && o.CancelledAt <= thresholdDate)
                .ToListAsync();

            if (oldCancelledOrders.Any())
            {
                _context.Orders.RemoveRange(oldCancelledOrders);
                await _context.SaveChangesAsync();
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
