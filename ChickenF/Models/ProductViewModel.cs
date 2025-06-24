using System.Collections.Generic;
namespace ChickenF.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int PricePerUnit { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public int TotalSold { get; set; } 
        public string ShortDescription {get; set; }
    }

}
