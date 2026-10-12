using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Goto.Infrastructure.Routing.Filters;

namespace Goto.Tests.Infrastructure.Routing.Filters;

[TestClass]
public class ValidateRequestAttributeTests
{
    public static ValidateRequestAttribute Attribute { get; set; } = new();

    [TestMethod]
    public void OnActionExecutingShouldThrowWhenModelStateInvalid()
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new (), new ());
        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());
        context.ModelState.AddModelError("field", "error message");

        var ex = Assert.Throws<BadHttpRequestException>(() => Attribute.OnActionExecuting(context));
        Assert.Contains("error message", ex.Message);
        Assert.AreEqual(400, ex.StatusCode);
    }
}
