using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Goto.Infrastructure.Routing.Binding;

public sealed class GS1ResolverModelBinderProvider : IModelBinderProvider
{
    private static readonly IConditionalModelBinder[] ConditionalBinders = [
        new FromUriModelBinder(),
        new FromHeaderModelBinder(),
        new FromQueryModelBinder()
    ];

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return ConditionalBinders.FirstOrDefault(binder => binder.CanBind(context));
    }
}
