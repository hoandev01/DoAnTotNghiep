using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChickenF.Models;

namespace ChickenF.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        public int BroodingDays { get; set; }
        public int GrowthDays { get; set; }
        public int PreSaleDays { get; set; }
        public int ReadyDays { get; set; }

        // ✅ Một Category có thể chứa nhiều Flock
        public ICollection<Flock> Flocks { get; set; } = new List<Flock>();

    }
}
