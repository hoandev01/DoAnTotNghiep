namespace ChickenF.Models
{
    public class SearchResultViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public object RouteValues { get; set; }
    }


}
