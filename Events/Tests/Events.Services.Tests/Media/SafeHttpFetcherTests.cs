using System.Net;
using Events.Models.Enums;
using Events.Services.Media;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Events.Services.Tests.Media;

public class SafeHttpFetcherTests
{
    private const string ImageUrl = "https://scontent.example.net/v/photo.jpg?oh=abc&oe=6A1B2C3D";

    [Fact]
    public async Task FetchImageAsync_Success_ReturnsBodyContentTypeAndFinalUri()
    {
        var handler = new FakeHandler(_ => Ok(new byte[] { 1, 2, 3 }, "image/jpeg"));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(new byte[] { 1, 2, 3 }, result.Content);
        Assert.Equal("image/jpeg", result.ContentType);
        Assert.Equal(new Uri(ImageUrl), result.FinalUri);
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("/relative/photo.jpg")]
    [InlineData("file:///etc/passwd")]
    [InlineData("")]
    public async Task FetchImageAsync_NotAbsoluteUrl_ReturnsInvalidUrl(string url)
    {
        var handler = new FakeHandler(_ => Ok(new byte[] { 1 }));

        var result = await CreateFetcher(handler).FetchImageAsync(url);

        Assert.Equal(ImageFetchFailureReason.InvalidUrl, result.FailureReason);
        Assert.Equal(0, handler.RequestCount);
    }

    [Theory]
    [InlineData("http://example.com/photo.jpg")]
    [InlineData("ftp://example.com/photo.jpg")]
    public async Task FetchImageAsync_NotHttps_RefusedWithoutRequest(string url)
    {
        var handler = new FakeHandler(_ => Ok(new byte[] { 1 }));

        var result = await CreateFetcher(handler).FetchImageAsync(url);

        Assert.Equal(ImageFetchFailureReason.NotHttps, result.FailureReason);
        Assert.Equal(0, handler.RequestCount);
    }

    [Theory]
    [InlineData("https://127.0.0.1/photo.jpg")]
    [InlineData("https://10.0.0.1/photo.jpg")]
    [InlineData("https://192.168.0.5/photo.jpg")]
    [InlineData("https://169.254.169.254/latest/meta-data")]
    [InlineData("https://[::1]/photo.jpg")]
    [InlineData("https://[::ffff:127.0.0.1]/photo.jpg")]
    [InlineData("https://localhost/photo.jpg")]
    [InlineData("https://example.com:8443/photo.jpg")]
    public async Task FetchImageAsync_ForbiddenTarget_RefusedWithoutRequest(string url)
    {
        var handler = new FakeHandler(_ => Ok(new byte[] { 1 }));

        var result = await CreateFetcher(handler).FetchImageAsync(url);

        Assert.Equal(ImageFetchFailureReason.ForbiddenAddress, result.FailureReason);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task FetchImageAsync_HostResolvesToForbiddenAddress_ReturnsForbiddenAddress()
    {
        // The connect step throws this when DNS returns an internal address; HttpClient wraps it.
        var handler = new FakeHandler(_ => throw new HttpRequestException(
            "Connection refused", new ForbiddenAddressException("internal.example.com")));

        var result = await CreateFetcher(handler).FetchImageAsync("https://internal.example.com/photo.jpg");

        Assert.Equal(ImageFetchFailureReason.ForbiddenAddress, result.FailureReason);
    }

    [Fact]
    public async Task FetchImageAsync_RedirectToInternalAddress_RefusedBeforeSecondRequest()
    {
        var handler = new FakeHandler(_ => Redirect("https://10.0.0.1/photo.jpg"));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.ForbiddenAddress, result.FailureReason);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task FetchImageAsync_RedirectToHttp_ReturnsNotHttps()
    {
        var handler = new FakeHandler(_ => Redirect("http://example.com/photo.jpg"));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.NotHttps, result.FailureReason);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task FetchImageAsync_ThreeRedirects_FollowsThemIncludingRelative()
    {
        var handler = new FakeHandler(request => request.RequestUri!.AbsolutePath switch
        {
            "/v/photo.jpg" => Redirect("https://cdn2.example.net/a.jpg"),
            "/a.jpg" => Redirect("/b.jpg"),
            "/b.jpg" => Redirect("https://cdn3.example.net/final.jpg"),
            _ => Ok(new byte[] { 9 })
        });

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(new Uri("https://cdn3.example.net/final.jpg"), result.FinalUri);
        Assert.Equal(4, handler.RequestCount);
        Assert.Equal(new Uri("https://cdn2.example.net/b.jpg"), handler.RequestedUris[2]);
    }

    [Fact]
    public async Task FetchImageAsync_FourRedirects_ReturnsHttpError()
    {
        var counter = 0;
        var handler = new FakeHandler(_ => Redirect($"https://cdn.example.net/{++counter}.jpg"));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.HttpError, result.FailureReason);
        Assert.Equal(SafeHttpFetcher.MaxRedirects + 1, handler.RequestCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, ImageFetchFailureReason.NotFound)]
    [InlineData(HttpStatusCode.Gone, ImageFetchFailureReason.NotFound)]
    [InlineData(HttpStatusCode.Forbidden, ImageFetchFailureReason.HttpError)]
    [InlineData(HttpStatusCode.InternalServerError, ImageFetchFailureReason.HttpError)]
    public async Task FetchImageAsync_ErrorStatus_MapsReasonAndKeepsStatus(HttpStatusCode status, ImageFetchFailureReason expected)
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(status));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(expected, result.FailureReason);
        Assert.Equal(status, result.StatusCode);
    }

    [Fact]
    public async Task FetchImageAsync_ContentLengthOverLimit_ReturnsTooLarge()
    {
        var handler = new FakeHandler(_ =>
        {
            var response = Ok(new byte[] { 1 });
            response.Content.Headers.ContentLength = SafeHttpFetcher.MaxImageBytes + 1;
            return response;
        });

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.TooLarge, result.FailureReason);
    }

    [Fact]
    public async Task FetchPageAsync_BodyOverLimitWithoutContentLength_ReturnsTooLarge()
    {
        var body = new byte[SafeHttpFetcher.MaxPageBytes + 1];
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new UnknownLengthContent(body) });

        var result = await CreateFetcher(handler).FetchPageAsync("https://www.example.com/event");

        Assert.Equal(ImageFetchFailureReason.TooLarge, result.FailureReason);
    }

    [Fact]
    public async Task FetchPageAsync_BodyAtLimitWithoutContentLength_Succeeds()
    {
        var body = new byte[SafeHttpFetcher.MaxPageBytes];
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new UnknownLengthContent(body) });

        var result = await CreateFetcher(handler).FetchPageAsync("https://www.example.com/event");

        Assert.True(result.IsSuccess);
        Assert.Equal(body.Length, result.Content.Length);
    }

    [Fact]
    public async Task FetchImageAsync_RequestTimesOut_ReturnsTimeout()
    {
        var handler = new FakeHandler(_ => throw new TaskCanceledException("Timed out", new TimeoutException()));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.Timeout, result.FailureReason);
    }

    [Fact]
    public async Task FetchImageAsync_CallerCancels_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var handler = new FakeHandler(_ => throw new TaskCanceledException());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateFetcher(handler).FetchImageAsync(ImageUrl, cts.Token));
    }

    [Fact]
    public async Task FetchImageAsync_NetworkError_ReturnsHttpError()
    {
        var handler = new FakeHandler(_ => throw new HttpRequestException("No such host is known."));

        var result = await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        Assert.Equal(ImageFetchFailureReason.HttpError, result.FailureReason);
    }

    [Fact]
    public async Task FetchImageAsync_SendsBrowserHeaders()
    {
        var handler = new FakeHandler(_ => Ok(new byte[] { 1 }));

        await CreateFetcher(handler).FetchImageAsync(ImageUrl);

        var request = handler.LastRequest!;
        Assert.Contains("Mozilla/5.0", request.Headers.UserAgent.ToString());
        Assert.Contains("image/webp", request.Headers.Accept.ToString());
    }

    private static SafeHttpFetcher CreateFetcher(HttpMessageHandler handler)
    {
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(SafeHttpFetcher.HttpClientName)).Returns(() => new HttpClient(handler, disposeHandler: false));
        return new SafeHttpFetcher(factory.Object, NullLogger<SafeHttpFetcher>.Instance);
    }

    private static HttpResponseMessage Ok(byte[] body, string contentType = "application/octet-stream")
    {
        var content = new ByteArrayContent(body);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
    }

    private static HttpResponseMessage Redirect(string location)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Found);
        response.Headers.Location = new Uri(location, UriKind.RelativeOrAbsolute);
        return response;
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            _respond = respond;
        }

        public int RequestCount => RequestedUris.Count;
        public List<Uri> RequestedUris { get; } = new();
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RequestedUris.Add(request.RequestUri!);
            LastRequest = request;
            return Task.FromResult(_respond(request));
        }
    }

    // Body without a known length (like a chunked response), so only the read limit can stop it.
    private sealed class UnknownLengthContent : HttpContent
    {
        private readonly byte[] _body;

        public UnknownLengthContent(byte[] body)
        {
            _body = body;
        }

        protected override Task SerializeToStreamAsync(Stream stream, System.Net.TransportContext? context) =>
            stream.WriteAsync(_body, 0, _body.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
