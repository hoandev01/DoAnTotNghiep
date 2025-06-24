using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class Tracking
    {
        public int Id { get; set; }

        [Required]
        public int FlockId { get; set; }

        [ForeignKey("FlockId")]
        public Flock? Flock { get; set; }

        [DataType(DataType.Date)]
        public DateTime TrackingDate { get; set; }

        public string? HealthStatus { get; set; }

        public float? Temperature { get; set; }

        public float? Humidity { get; set; }

        public int FeedCost { get; set; }



        public string? Note { get; set; }
    }
}
