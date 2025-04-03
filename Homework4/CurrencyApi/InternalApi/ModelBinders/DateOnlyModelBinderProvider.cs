using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fuse8.BackendInternship.InternalApi.ModelBinders;

public class DateOnlyModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(DateOnly))
        {
            return new BinderTypeModelBinder(typeof(DateOnlyModelBinder));
        }

        return null;
    }
}
