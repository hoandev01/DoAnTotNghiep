using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class Flock
    {
        [Key]
        public int Id { get; set; } 
        [Required, StringLength(100)]
        public string FlockName { get; set; } = string.Empty;

        

        [Required, ForeignKey("Cage")]
        public int CageId { get; set; }
        public Cage? Cage { get; set; }

        [Required, ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int FlockQuantity { get; set; }

        public string? FlockNote { get; set; }

        [Required]
        public string? ChickenSize { get; set; }        // e.g., "small", "medium", "large"

        [Required]
        public string? FeedType { get; set; }  // e.g., "organic", "industrial"

        [Required]
        public string? Status { get; set; }            // 1 - still raising, 0 - sold

        public DateTime DayIn { get; set; }
        public string? GrowthLevel { get; set; }          // e.g., "good", "average"

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<FlockStage>? FlockStages { get; set; }

        public ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();

    }
}
