using System.Collections.Generic;
namespace ChickenF.Models
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string ImageNews { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
    }

}
