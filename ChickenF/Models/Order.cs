using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        
        public User User { get; set; }
        public string? CancelReason { get; set; }
        public bool? IsReviewed { get; set; } // Dành cho đánh giá sau nhận hàng
        public DateTime? CancelledAt { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
