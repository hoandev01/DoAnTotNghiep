using System.Collections.Generic;
namespace ChickenF.Models
{
    public class EmployeeDashboardViewModel
    {
        public List<TabViewModel> Tabs { get; set; } = new();
    }

    public class TabViewModel
    {
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";
        public List<string> Headers { get; set; } = new();
        public List<RowViewModel> Items { get; set; } = new();
    }

    public class RowViewModel
    {
        public List<object> Values { get; set; } = new();
        public string EditUrl { get; set; } = "";
        public string DetailsUrl { get; set; } = "";
        public string DeleteUrl { get; set; } = "";
    }

}
