using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class LargeStringValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.LargeString;

    public void Validate(object? value)
    {
        if (value is null)
            throw new FeatureKeyValidationException("LargeString value cannot be null.");

        if (value is string)
            return;

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
                return;

            throw new FeatureKeyValidationException(
                $"LargeString value must be a string. Got JSON '{jsonElement.ValueKind}'.");
        }

        throw new FeatureKeyValidationException(
            $"LargeString value must be a string. Got '{value.GetType().Name}'.");
    }
}
