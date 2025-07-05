using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Flock")]
        public int FlockId { get; set; }
        public Flock? Flock { get; set; }


        [Required, StringLength(100)]
        public string ProductName { get; set; } = string.Empty;
        public string? Image { get; set; }

        
        public int Price { get; set; }



        public int ReservedQuantity { get; set; } = 0;
        public int ProductStock { get; set; }
        [DataType(DataType.Date)]
        
        public DateTime? DateCreated { get; set; }

        public string? ProductDescription { get; set; }

        public DateTime? OutOfStockAt { get; set; }


        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }
}
