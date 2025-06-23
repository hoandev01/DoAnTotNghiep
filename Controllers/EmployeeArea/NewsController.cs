using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using ChickenF.Data;
using ChickenF.Models;
using HtmlAgilityPack; // ✅ Gói thư viện xử lý HTML an toàn

namespace ChickenF.Controllers.EmployeeArea
{
    public class NewsController : Controller
    {
        private readonly FarmContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public NewsController(FarmContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var localNews = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();

            return View(localNews);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return NotFound();
            return View(article);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticle news)
        {
            if (ModelState.IsValid)
            {
                news.PublishedDate = DateTime.Now;
                _context.NewsArticles.Add(news);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Added News Successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(news);
        }

        public async Task<IActionResult> ImportFromRss()
        {
           
            await FetchFromRssFeedsAsync();
            await FetchFromHtmlBlogsAsync();
            TempData["Success"] = "Imported and saved news from VnExpress.";
            return RedirectToAction("Index");
        }

        private async Task FetchFromRssFeedsAsync()
        {
            string[] rssUrls = new[]
            {
        
        "https://www.meatpoultry.com/rss/topic/305-poultry.xml"
    };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            foreach (var url in rssUrls)
            {
                try
                {
                    var response = await client.GetStringAsync(url);
                    response = WebUtility.HtmlDecode(response);
                    response = Regex.Replace(response, "&(?!(amp|lt|gt|apos|quot);)", "&amp;");

                    var xdoc = System.Xml.Linq.XDocument.Parse(response);
                    var items = xdoc.Descendants("item");

                    foreach (var item in items)
                    {
                        try
                        {
                            var title = item.Element("title")?.Value;
                            if (string.IsNullOrEmpty(title)) continue;
                            if (await _context.NewsArticles.AnyAsync(n => n.Title == title)) continue;

                            var descHtml = item.Element("description")?.Value ?? "";
                            var pubDateRaw = item.Element("pubDate")?.Value;

                            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(descHtml);

                            var textContent = htmlDoc.DocumentNode.InnerText.Trim();
                            var imageUrl = htmlDoc.DocumentNode.SelectSingleNode("//img")?.GetAttributeValue("src", null);

                            var article = new NewsArticle
                            {
                                Title = title,
                                Summary = textContent,
                                Content = textContent,
                                ImageNews = imageUrl,
                                PublishedDate = DateTime.TryParse(pubDateRaw, out var date)
                                                ? date : DateTime.Now
                            };

                            _context.NewsArticles.Add(article);
                            Console.WriteLine($"✔ RSS: {title}");
                        }
                        catch (Exception exItem)
                        {
                            Console.WriteLine($"⚠️ RSS item error: {exItem.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error reading RSS '{url}': {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
        }
        private async Task FetchFromHtmlBlogsAsync()
        {
            string[] blogUrls = new[]
            {
        "https://www.fresheggsdaily.blog/",
        "https://www.backyardchickens.com/articles/",
        "https://blog.meyerhatchery.com/all-posts/",
        // Bạn có thể thêm nhiều blog HTML khác tại đây
    };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            foreach (var blogUrl in blogUrls)
            {
                try
                {
                    var html = await client.GetStringAsync(blogUrl);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    // ✅ Xác định cách tìm node tiêu đề bài viết tùy theo từng trang
                    var postNodes = blogUrl.Contains("fresheggsdaily")
                        ? doc.DocumentNode.SelectNodes("//h2/a[@href]")
                        : blogUrl.Contains("backyardchickens")
                            ? doc.DocumentNode.SelectNodes("//a[contains(@class,'article-preview-title')]")
                            : null;

                    if (postNodes == null) continue;

                    foreach (var node in postNodes)
                    {
                        try
                        {
                            var link = node.GetAttributeValue("href", null);
                            var title = WebUtility.HtmlDecode(node.InnerText.Trim());

                            // Bổ sung prefix nếu href là đường dẫn tương đối
                            if (!string.IsNullOrEmpty(link) && link.StartsWith("/"))
                                link = blogUrl.TrimEnd('/') + link;

                            if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(title)) continue;
                            if (await _context.NewsArticles.AnyAsync(n => n.Title == title)) continue;

                            // Lấy nội dung bài viết cụ thể
                            var postHtml = await client.GetStringAsync(link);
                            var postDoc = new HtmlAgilityPack.HtmlDocument();
                            postDoc.LoadHtml(postHtml);

                            HtmlNode articleContentNode = null;
                            if (blogUrl.Contains("fresheggsdaily"))
                            {
                                articleContentNode = postDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'post-body')]");
                            }
                            else if (blogUrl.Contains("backyardchickens"))
                            {
                                articleContentNode = postDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'bbWrapper')]");
                            }

                            var contentText = articleContentNode?.InnerText.Trim() ?? "";

                            var imgNode = articleContentNode?.SelectSingleNode(".//img");
                            var imageUrl = imgNode?.GetAttributeValue("src", null);

                            var article = new NewsArticle
                            {
                                Title = title,
                                Summary = contentText.Substring(0, Math.Min(200, contentText.Length)).Trim(),
                                Content = contentText,
                                ImageNews = imageUrl,
                                PublishedDate = DateTime.Now
                            };

                            _context.NewsArticles.Add(article);
                            Console.WriteLine($"✔ Scraped: {title}");
                        }
                        catch (Exception innerEx)
                        {
                            Console.WriteLine($"⚠️ Skip article error: {innerEx.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error scraping blog '{blogUrl}': {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsArticle updatedNews)
        {
            if (id != updatedNews.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.NewsArticles.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Title = updatedNews.Title;
                    existing.Summary = updatedNews.Summary;
                    existing.Content = updatedNews.Content;
                    existing.ImageNews = updatedNews.ImageNews;
                    existing.PublishedDate = DateTime.Now;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Updated news successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NewsArticles.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
            }

            return View(updatedNews);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return NotFound();

            return View(article); // Hoặc return RedirectToAction("Index") nếu bạn không muốn xác nhận
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return NotFound();

            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Deleted news successfully.";
            return RedirectToAction(nameof(Index));
        }
        

    }

}
