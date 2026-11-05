using Goto.Services.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Goto.Infrastructure.Routing.Binding;

internal class FromQueryModelBinder : IConditionalModelBinder
{
    public string? Name { get; set; }

    public bool CanBind(ModelBinderProviderContext context)
    {
        return context.Metadata.UnderlyingOrModelType == typeof(LinkType)
            && (context.BindingInfo.BindingSource?.CanAcceptDataFrom(BindingSource.Query) ?? false);
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;

        if (bindingContext.ModelType == typeof(LinkType))
        {
            var queryParameter = request.Query["linkType"];
            var linkType = queryParameter.Where(p => p is not null).LastOrDefault(string.Empty);

            bindingContext.Result = ModelBindingResult.Success(new LinkType(linkType!));
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}