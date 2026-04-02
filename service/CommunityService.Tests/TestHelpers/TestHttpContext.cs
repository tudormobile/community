using System.Net;
using Microsoft.AspNetCore.Http.Features;

namespace CommunityService.Tests.TestHelpers;

/// <summary>
/// Simple test HTTP context for unit testing.
/// </summary>
internal class TestHttpContext : HttpContext
{
    private readonly TestConnectionInfo _connection = new();
    private readonly TestHttpRequest _request = new();
    private readonly TestHttpResponse _response = new();
    private readonly Dictionary<object, object?> _items = [];

    public override IFeatureCollection Features { get; } = null!;
    public override HttpRequest Request => _request;
    public override HttpResponse Response => _response;
    public override ConnectionInfo Connection => _connection;
    public override WebSocketManager WebSockets { get; } = null!;
    public override ClaimsPrincipal User { get; set; } = new();
    public override IDictionary<object, object?> Items { get => _items; set { } }
    public override IServiceProvider RequestServices { get; set; } = null!;
    public override CancellationToken RequestAborted { get; set; }
    public override string TraceIdentifier { get; set; } = string.Empty;
    public override ISession Session { get; set; } = null!;

    public override void Abort() { }
}

internal class TestConnectionInfo : ConnectionInfo
{
    public override string Id { get; set; } = string.Empty;
    public override IPAddress? RemoteIpAddress { get; set; } = IPAddress.Parse("127.0.0.1");
    public override int RemotePort { get; set; }
    public override IPAddress? LocalIpAddress { get; set; }
    public override int LocalPort { get; set; }
    public override X509Certificate2? ClientCertificate { get; set; }
    public override Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<X509Certificate2?>(null);
}

internal class TestHttpRequest : HttpRequest
{
    public override HttpContext HttpContext { get; } = null!;
    public override string Method { get; set; } = "GET";
    public override string Scheme { get; set; } = "http";
    public override bool IsHttps { get; set; }
    public override HostString Host { get; set; }
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IQueryCollection Query { get; set; } = null!;
    public override string Protocol { get; set; } = "HTTP/1.1";
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override IRequestCookieCollection Cookies { get; set; } = null!;
    public override long? ContentLength { get; set; }
    public override string? ContentType { get; set; }
    public override Stream Body { get; set; } = Stream.Null;
    public override bool HasFormContentType => false;
    public override IFormCollection Form { get; set; } = null!;
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IFormCollection>(null!);
}

internal class TestHttpResponse : HttpResponse
{
    public override HttpContext HttpContext { get; } = null!;
    public override int StatusCode { get; set; }
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override Stream Body { get; set; } = Stream.Null;
    public override long? ContentLength { get; set; }
    public override string? ContentType { get; set; }
    public override IResponseCookies Cookies { get; } = null!;
    public override bool HasStarted => false;
    public override void OnStarting(Func<object, Task> callback, object state) { }
    public override void OnCompleted(Func<object, Task> callback, object state) { }
    public override void Redirect(string location, bool permanent) { }
}
