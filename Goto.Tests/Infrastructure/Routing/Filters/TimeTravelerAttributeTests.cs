using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Goto.Infrastructure.Routing.Filters;
using Goto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Goto.Tests.Infrastructure.Routing.Filters;

[TestClass]
public class TimeTravelerAttributeTests
{
    public static TimeTravelerAttribute Attribute { get; set; } = new();

    [TestMethod]
    public void ShouldThrowIfHeaderDateIsInFuture()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Request-Date"] = DateTimeOffset.UtcNow.AddDays(1).ToString("o");

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        Assert.Throws<InvalidOperationException>(() => Attribute.OnActionExecuting(context));
    }

    [TestMethod]
    public void ShouldSetClockWhenHeaderDateIsValidPast()
    {
        var date = DateTimeOffset.UtcNow.AddHours(-2);
        var provider = new ServiceCollection().AddSingleton<Clock>().BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        
        httpContext.Request.Headers["X-Request-Date"] = date.ToString("o");

        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        Attribute.OnActionExecuting(context);

        var clock = provider.GetRequiredService<Clock>();
        Assert.AreEqual(date, clock.Now);
    }
}
