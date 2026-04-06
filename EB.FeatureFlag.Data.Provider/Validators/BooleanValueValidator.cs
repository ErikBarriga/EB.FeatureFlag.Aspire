using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class BooleanValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.Boolean;

    public void Validate(object? value, string? validationRegex = null)
    {
        if (value is null)
            throw new FeatureKeyValidationException("Boolean value cannot be null.");

        if (value is bool)
            return;

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind is JsonValueKind.True or JsonValueKind.False)
                return;

            throw new FeatureKeyValidationException(
                $"Boolean value must be true or false. Got JSON '{jsonElement.ValueKind}'.");
        }

        if (value is string str && bool.TryParse(str, out _))
            return;

        throw new FeatureKeyValidationException(
            $"Boolean value must be true or false. Got '{value.GetType().Name}'.");
    }
}
