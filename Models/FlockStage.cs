using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // THÊM namespace này
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class FlockStage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Flock field is required.")]
        [Display(Name = "Flock")]
        public int FlockId { get; set; }

        [ForeignKey("FlockId")]
        [ValidateNever] 
        public Flock Flock { get; set; }

        [Required]
        [StringLength(100)]
        public string StageName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Note { get; set; }
    }
}
