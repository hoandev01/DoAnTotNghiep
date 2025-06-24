using System.Drawing;
using System.Collections.Generic;
namespace ChickenF.Models
{
    public class HomeViewModel
    {

        public List<ProductViewModel> FeaturedProducts { get; set; }
        public List<NewsArticle> LatestNewsArticles { get; set; }
        public List<Slide> Slides { get; set; }
        public List<ProductViewModel> TopSellingProducts{get; set;}
    }
}
