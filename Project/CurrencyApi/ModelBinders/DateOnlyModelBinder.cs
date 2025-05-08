using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Fuse8.BackendInternship.ModelBinders;

public class DateOnlyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            bindingContext.ModelState.TryAddModelError(modelName, "Некорректная дата. Формат yyyy-MM-dd\"");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(dateOnly);
        return Task.CompletedTask;
    }
}
