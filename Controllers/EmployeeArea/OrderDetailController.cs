using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChickenF.Data;
using ChickenF.Models;
using System.Linq;
using System.Threading.Tasks;
using ChickenF;
using System.Threading;


public class OrderDetailsController : Controller
{
    private readonly FarmContext _context;

    public OrderDetailsController(FarmContext context)
    {
        _context = context;
    }

    // GET: OrderDetails
    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 5;

        var orderDetails =  _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .AsNoTracking()
            .OrderByDescending(f => f.OrderId);
        var paginatedList = await PaginatedList<OrderDetail>.CreateAsync(orderDetails, page, pageSize);
        return View(paginatedList);
    }

    // GET: OrderDetails/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: OrderDetails/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderDetail orderDetail)
    {
        if (ModelState.IsValid)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(orderDetail);
    }

    // GET: OrderDetails/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail == null)
        {
            return NotFound();
        }
        return View(orderDetail);
    }

    // POST: OrderDetails/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderDetail orderDetail)
    {
        if (id != orderDetail.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(orderDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(orderDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(orderDetail);
    }

    // GET: OrderDetails/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var orderDetail = await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderDetail == null)
        {
            return NotFound();
        }

        return View(orderDetail);
    }

    // POST: OrderDetails/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail != null)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }
        else
        {
            return NotFound();
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderDetailExists(int id)
    {
        return _context.OrderDetails.Any(e => e.Id == id);
    }
}
