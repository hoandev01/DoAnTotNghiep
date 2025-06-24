using System;
using ChickenF.Models; // Đảm bảo namespace này đúng với Flock, Cage, Category, v.v.

namespace ChickenF.Models.ViewModels
{
    public class FlockReportViewModel
    {
        public Flock Flock { get; set; }

        public int TotalRaised { get; set; }
        public int TotalSold { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalFeedCost { get; set; }

        // Read-only property tính toán từ 2 giá trị trên
        public decimal Profit => TotalRevenue - TotalFeedCost;

        public DateTime ReportGeneratedAt { get; set; }
    }
}
