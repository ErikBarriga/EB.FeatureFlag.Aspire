using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class StringCollectionValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.StringCollection;

    public void Validate(object? value)
    {
        if (value is null)
            throw new FeatureKeyValidationException("StringCollection value cannot be null.");

        if (value is IEnumerable<string>)
            return;

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind != JsonValueKind.Array)
                throw new FeatureKeyValidationException(
                    $"StringCollection value must be an array. Got JSON '{jsonElement.ValueKind}'.");

            foreach (var item in jsonElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.String)
                    throw new FeatureKeyValidationException(
                        $"StringCollection elements must be strings. Got JSON '{item.ValueKind}'.");
            }

            return;
        }

        throw new FeatureKeyValidationException(
            $"StringCollection value must be an array of strings. Got '{value.GetType().Name}'.");
    }
}
