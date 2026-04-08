using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class JsonCollectionValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.JsonCollection;

    public void Validate(object? value, string? validationRegex = null)
    {
        if (value is null)
            throw new FeatureKeyValidationException("JsonCollection value cannot be null.");

        if (value is JsonElement jsonElement)
        {
            // Accept both direct JSON arrays and JSON strings that contain an array
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                ValidateJsonElementArray(jsonElement);
                return;
            }

            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                var inner = jsonElement.GetString();
                if (string.IsNullOrWhiteSpace(inner))
                    throw new FeatureKeyValidationException("JsonCollection value must be a JSON array.");

                try
                {
                    using var doc = JsonDocument.Parse(inner);
                    Validate(doc.RootElement, validationRegex);
                    return;
                }
                catch (JsonException)
                {
                    throw new FeatureKeyValidationException("JsonCollection value must be a valid JSON array.");
                }
            }

            throw new FeatureKeyValidationException(
                $"JsonCollection value must be a JSON array or JSON string containing an array. Got JSON '{jsonElement.ValueKind}'.");
        }

        if (value is string rawJson)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                Validate(doc.RootElement, validationRegex);
                return;
            }
            catch (JsonException)
            {
                throw new FeatureKeyValidationException(
                    "JsonCollection value must be a valid JSON array.");
            }
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

    private static void ValidateJsonElementArray(JsonElement jsonElement)
    {
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
