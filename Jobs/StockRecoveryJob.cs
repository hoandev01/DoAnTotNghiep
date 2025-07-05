using ChickenF.Data;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Jobs
{
    public class StockRecoveryJob
    {
        private readonly FarmContext _context;

        public StockRecoveryJob(FarmContext context)
        {
            _context = context;
        }

        public async Task RestoreStockFromCancelledOrders()
        {
            var cancelledOrders = await _context.Orders
                .Where(o => o.Status == "Cancelled" && !o.StockRestored)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();

            int updated = 0;

            foreach (var order in cancelledOrders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        // Giảm ReservedQuantity
                        detail.Product.ReservedQuantity = Math.Max(
                            0,
                            detail.Product.ReservedQuantity - detail.OrderDetailQuantity
                        );
                        updated++;
                    }
                }

                order.StockRestored = true;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"[StockRecoveryJob] Đã cập nhật {updated} dòng ReservedQuantity từ các đơn bị hủy.");
        }
    }

}
