using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Events.Crawler.Functions;

public class SimpleTestFunction
{
    private readonly ILogger<SimpleTestFunction> _logger;

    public SimpleTestFunction(ILogger<SimpleTestFunction> logger)
    {
        _logger = logger;
    }

    [Function("SimpleTest")]
    public async Task<HttpResponseData> SimpleTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        _logger.LogInformation("SimpleTest function executed!");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        await response.WriteStringAsync("{\"message\": \"Function App is working!\", \"timestamp\": \"" + DateTime.UtcNow + "\"}");
        return response;
    }
}