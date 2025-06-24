namespace ChickenF.Models
{
    public class FlockViewModel
    {
        public Flock Flock { get; set; }
        public string CurrentStageName { get; set; } // Đang ở giai đoạn nào
        public string StatusMessage { get; set; } // Gần đến ngày xuất chuồng chưa

        public DateTime? SuggestedSaleDate { get; set; }
    }
}
