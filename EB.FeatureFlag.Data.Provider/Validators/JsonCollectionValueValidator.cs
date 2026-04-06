using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class JsonCollectionValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.JsonCollection;

    public void Validate(object? value)
    {
        if (value is null)
            throw new FeatureKeyValidationException("JsonCollection value cannot be null.");

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind != JsonValueKind.Array)
                throw new FeatureKeyValidationException(
                    $"JsonCollection value must be an array. Got JSON '{jsonElement.ValueKind}'.");

            var index = 0;
            foreach (var item in jsonElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var jsonString = item.GetString()!;
                    ValidateJsonString(jsonString, index);
                }
                else if (item.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
                {
                    throw new FeatureKeyValidationException(
                        $"JsonCollection element at index {index} must be a JSON object, array, or valid JSON string. Got '{item.ValueKind}'.");
                }

                index++;
            }

            return;
        }

        if (value is IEnumerable<string> strings)
        {
            var index = 0;
            foreach (var item in strings)
            {
                ValidateJsonString(item, index);
                index++;
            }

            return;
        }

        throw new FeatureKeyValidationException(
            $"JsonCollection value must be an array. Got '{value.GetType().Name}'.");
    }

    private static void ValidateJsonString(string jsonString, int index)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonString);
        }
        catch (JsonException)
        {
            throw new FeatureKeyValidationException(
                $"JsonCollection element at index {index} is not valid JSON: '{jsonString}'.");
        }
    }
}
