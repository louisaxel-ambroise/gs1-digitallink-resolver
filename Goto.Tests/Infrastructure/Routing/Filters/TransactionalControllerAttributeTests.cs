using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Goto.Infrastructure.Routing.Filters;
using Goto.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Goto.Tests.Infrastructure.Routing.Filters;

[TestClass]
public class TransactionalControllerAttributeTests
{
    public static TransactionalControllerAttribute Attribute { get; set; } = new();

    [TestMethod]
    [DataRow("CONNECT", QueryTrackingBehavior.NoTracking)]
    [DataRow("TRACE", QueryTrackingBehavior.NoTracking)]
    [DataRow("GET", QueryTrackingBehavior.NoTracking)]
    [DataRow("HEAD", QueryTrackingBehavior.NoTracking)]
    [DataRow("OPTIONS", QueryTrackingBehavior.NoTracking)]
    [DataRow("QUERY", QueryTrackingBehavior.NoTracking)]
    [DataRow("POST", QueryTrackingBehavior.TrackAll)]
    [DataRow("PATCH", QueryTrackingBehavior.TrackAll)]
    [DataRow("PUT", QueryTrackingBehavior.TrackAll)]
    [DataRow("DELETE", QueryTrackingBehavior.TrackAll)]
    public void OnActionExecutingShouldSetTheQueryTrackingBehavior(string method, QueryTrackingBehavior expectedBehavior)
    {
        var databaseContext = new Context(new DbContextOptionsBuilder<Context>().UseSqlite("Data Source=:memory:").Options, new());
        var provider = new ServiceCollection().AddScoped(_ => databaseContext).BuildServiceProvider();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = method;
        httpContext.RequestServices = provider;
        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        Attribute.OnActionExecuting(context);

        Assert.AreEqual(expectedBehavior, databaseContext.ChangeTracker.QueryTrackingBehavior);
    }

    [TestMethod]
    [DataRow("CONNECT", false)]
    [DataRow("TRACE", false)]
    [DataRow("GET", false)]
    [DataRow("HEAD", false)]
    [DataRow("OPTIONS", false)]
    [DataRow("QUERY", false)]
    [DataRow("POST", true)]
    [DataRow("PATCH", true)]
    [DataRow("PUT", true)]
    [DataRow("DELETE", true)]
    public void OnActionExecutedShouldSaveTheChangesForWriteHttpMethods(string method, bool expectedChangesSaved)
    {
        var changesSaved = false;
        var databaseContext = new Context(new DbContextOptionsBuilder<Context>().UseSqlite("Data Source=:memory:").Options, new());
        databaseContext.SavingChanges += (_, _) => { changesSaved = true; };

        var provider = new ServiceCollection().AddScoped(_ => databaseContext).BuildServiceProvider();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = method;
        httpContext.RequestServices = provider;
        var actionContext = new ActionContext(httpContext, new(), new());
        var context = new ActionExecutedContext(actionContext, [], new object());

        Attribute.OnActionExecuted(context);

        Assert.AreEqual(expectedChangesSaved, changesSaved);
    }
}
