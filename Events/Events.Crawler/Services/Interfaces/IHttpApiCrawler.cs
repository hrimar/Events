namespace Events.Crawler.Services.Interfaces;

public interface IHttpApiCrawler : IEventCrawlerStrategy
{
    Task<T> GetDataAsync<T>(string endpoint, Dictionary<string, string>? parameters = null);
}