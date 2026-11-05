using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Goto.Infrastructure.Routing.Binding;

public interface IConditionalModelBinder : IModelBinder
{
    bool CanBind(ModelBinderProviderContext context);
}
