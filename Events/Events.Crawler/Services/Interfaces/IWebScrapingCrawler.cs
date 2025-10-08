namespace Events.Crawler.Services.Interfaces;

public interface IWebScrapingCrawler : IEventCrawlerStrategy
{
    Task<IEnumerable<string>> ExtractElementsAsync(string url, string selector);
    Task<string> GetPageContentAsync(string url);
}
