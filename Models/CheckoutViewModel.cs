using System.Collections.Generic;
namespace ChickenF.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
