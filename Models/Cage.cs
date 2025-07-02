using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class Cage
    {
        [Key]
        
        public int Id { get; set; } 

        [Required, StringLength(100), Display(Name = "CageName")]
        public string CageName { get; set; } = string.Empty;

        [Required, Display(Name = "CageType")]
        public string CageType { get; set; } = "A"; // e.g., "A", "B"

        [Required, Range(1, int.MaxValue), Display(Name = "CageCapacity")]
        public int CageCapacity { get; set; }

        [StringLength(100), Display(Name = "CageArea")]
        public float CageArea { get; set; } // e.g., "500m2"
    }
}
