using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Goto.Infrastructure.Routing.Filters;
using Goto.Services.Data.Entities;
using Goto.Infrastructure.Results;
using Goto.Controllers.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Goto.Services;

namespace Goto.Tests;

[TestClass]
public class InsightsTrackingAttributeTests
{
    public static InsightsTrackingAttribute InsightsTracking => new();
    private static ServiceProvider BuildServices(Channel<Insight> channel)
    {
        return new ServiceCollection()
            .AddSingleton(channel)
            .AddSingleton(channel)
            .AddSingleton<Clock>()
            .BuildServiceProvider();
    }

    [TestMethod]
    public void ShouldNotWriteWhenRequestTrackingHeaderIsSetToBypass()
    {
        var channel = Channel.CreateUnbounded<Insight>();
        var provider = BuildServices(channel);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        httpContext.Request.Headers["X-Request-Tracking"] = "bypass";

        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var execContext = new ActionExecutedContext(actionContext, [], new())
        {
            Result = new OkObjectResult(null)
        };

        InsightsTracking.OnActionExecuted(execContext);

        Assert.IsFalse(channel.Reader.TryRead(out _));
    }

    [TestMethod]
    public void ShouldTrackMultipleChoicesResponse()
    {
        var channel = Channel.CreateUnbounded<Insight>();
        var provider = BuildServices(channel);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        httpContext.Request.Headers["Accept"] = "*/*";
        httpContext.Request.Headers["Accept-Language"] = "en-GB";
        httpContext.Items["gs1:digitalLink"] = "01/123";
        httpContext.Items["gs1:gcp"] = "5414195";
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("test.local");
        httpContext.Request.Path = "/01/123";

        var resolution = new ResolutionResult { Links = [new ResolutionResultLink { LinkType = "gs1:pip", Href = "h", Title = "tt" }, new ResolutionResultLink { LinkType = "t2", Href = "h2", Title = "tt2" }], Anchor = "a", Description = "d" };

        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var actionExecutedContext = new ActionExecutedContext(actionContext, [], new object())
        {
            Result = new MultipleChoicesObjectResult(resolution)
        };

        InsightsTracking.OnActionExecuted(actionExecutedContext);

        Assert.IsTrue(channel.Reader.TryRead(out var insight));
        Assert.AreEqual(300, insight.StatusCode);
        Assert.AreEqual(2, insight.LinkCount);
        Assert.AreEqual("5414195", insight.CompanyPrefix);
        Assert.AreEqual("01/123", insight.DigitalLink);
        Assert.Contains("/01/123", insight.Url);
        Assert.AreEqual("*/*", insight.Accept);
        Assert.AreEqual("en-GB", insight.AcceptLanguage);
    }

    [TestMethod]
    public void ShouldTrackRedirectResult()
    {
        var channel = Channel.CreateUnbounded<Insight>();
        var provider = BuildServices(channel);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        // Redirect
        var actionExecutedContext = new ActionExecutedContext(actionContext, [], new())
        {
            Result = new RedirectResult("https://redirect.url")
        };

        InsightsTracking.OnActionExecuted(actionExecutedContext);
        Assert.IsTrue(channel.Reader.TryRead(out var item));
        Assert.AreEqual(307, item.StatusCode);
        Assert.AreEqual(1, item.LinkCount);
    }

    [TestMethod]
    public void ShouldTrackNotFoundResult()
    {
        var channel = Channel.CreateUnbounded<Insight>();
        var provider = BuildServices(channel);
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        var actionExecutedContext = new ActionExecutedContext(actionContext, [], new())
        {
            Result = new NotFoundObjectResult(null)
        };

        InsightsTracking.OnActionExecuted(actionExecutedContext);
        Assert.IsTrue(channel.Reader.TryRead(out var item));
        Assert.AreEqual(404, item.StatusCode);
        Assert.AreEqual(0, item.LinkCount);
    }

    [TestMethod]
    public void ShouldTrackOkResult()
    {
        var channel = Channel.CreateUnbounded<Insight>();
        var provider = BuildServices(channel);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        var actionExecutedContext = new ActionExecutedContext(actionContext, [], new())
        {
            Result = new OkObjectResult(null)
        };
        
        InsightsTracking.OnActionExecuted(actionExecutedContext);
        Assert.IsTrue(channel.Reader.TryRead(out var item));
        Assert.AreEqual(500, item.StatusCode);
    }
}
