using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Goto.Infrastructure.Routing.Binding;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class FromUriAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Custom;
}
