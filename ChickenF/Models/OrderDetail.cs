using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderDetailQuantity { get; set; }
        public int OrderDetailPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }

       
        
     

    }
}
