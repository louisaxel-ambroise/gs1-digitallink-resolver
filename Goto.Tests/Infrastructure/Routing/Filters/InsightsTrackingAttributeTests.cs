using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Goto.Infrastructure.Routing.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Goto.Services.Data.Entities;
using Goto.Services;
using Goto.Infrastructure.Results;
using Goto.Controllers.Results;

namespace Goto.Tests.Infrastructure.Routing.Filters;

[TestClass]
public class InsightsTrackingAttributeTests
{
    public static InsightsTrackingAttribute Attribute { get; set; } = new();

    [TestMethod]
    [DataRow("bypass", true)]
    [DataRow("notrack", true)]
    [DataRow("", false)]
    [DataRow("track", false)]
    public void ShouldBypassTrackingOnlyWhenHeaderHasPredefinedValue(string headerValue, bool shouldBypass)
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Request-Tracking"] = headerValue;
        httpContext.RequestServices = provider;

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object());

        Attribute.OnActionExecuted(context);

        Assert.AreEqual(shouldBypass, channel.Reader.Count is 0);
    }

    [TestMethod]
    public void ShouldGetDigitalLinkValueFromContextItems()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };
        httpContext.Items.Add("gs1:digitalLink", "01/05414195535264");

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object());

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual("01/05414195535264", insight.DigitalLink);
    }

    [TestMethod]
    public void ShouldGetCompanyPrefixFromContextItems()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };
        httpContext.Items.Add("gs1:gcp", "0541419");

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object());

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual("0541419", insight.CompanyPrefix);
    }

    [TestMethod]
    public void ShouldMapFromMultipleChoicesResult()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };
        httpContext.Items.Add("gs1:gcp", "0541419");

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object())
        {
            Result = new MultipleChoicesObjectResult(new ResolutionResult
            {
                Anchor = "01/05414195535264",
                Description = "Test description",
                Links = [new() { Href = "https://test.com", LinkType = "gs1:test", Title = "test" }, new() { Href = "https://test.com", LinkType = "gs1:test", Title = "test" }]
            })
        };

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual(2, insight.LinkCount);
        Assert.AreEqual(300, insight.StatusCode);
    }

    [TestMethod]
    public void ShouldMapFromRedirectResult()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object())
        {
            Result = new RedirectResult("https://test.com")
        };

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual(1, insight.LinkCount);
        Assert.AreEqual(307, insight.StatusCode);
    }

    [TestMethod]
    public void ShouldMapFromNotFoundObjectResult()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object())
        {
            Result = new NotFoundObjectResult(new())
        };

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual(0, insight.LinkCount);
        Assert.AreEqual(404, insight.StatusCode);
    }

    [TestMethod]
    public void ShouldMapFromUnknownResult()
    {
        var channel = Channel.CreateBounded<Insight>(1);
        var provider = new ServiceCollection().AddSingleton(channel).AddSingleton(new Clock()).BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object());

        Attribute.OnActionExecuted(context);

        var insight = channel.Reader.TryRead(out var readInsight) ? readInsight : null;
        Assert.IsNotNull(insight);
        Assert.AreEqual(0, insight.LinkCount);
        Assert.AreEqual(500, insight.StatusCode);
    }
}