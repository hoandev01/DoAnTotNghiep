namespace ChickenF.Models
{
    public class AdminDashboardViewModel
    {
        public List<User> Customers { get; set; } = new();
        public List<User> Employees { get; set; } = new();
    }
}
