using System.Text.Json;
using System.Text.RegularExpressions;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class StringCollectionValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.StringCollection;

    public void Validate(object? value, string? validationRegex = null)
    {
        if (value is null)
            throw new FeatureKeyValidationException("StringCollection value cannot be null.");

        Regex? regex = CompileRegex(validationRegex);

        if (value is IEnumerable<string> strings)
        {
            ValidateItems(strings, regex, validationRegex);
            return;
        }

        if (value is JsonElement jsonElement)
        {
            // Accept both direct JSON arrays and JSON strings containing an array
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                ValidateJsonElement(jsonElement, regex, validationRegex);
                return;
            }

            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                var inner = jsonElement.GetString();
                if (string.IsNullOrWhiteSpace(inner))
                    throw new FeatureKeyValidationException("StringCollection value must be a JSON array of strings.");

                try
                {
                    using var doc = JsonDocument.Parse(inner);
                    ValidateJsonElement(doc.RootElement, regex, validationRegex);
                    return;
                }
                catch (JsonException)
                {
                    throw new FeatureKeyValidationException("StringCollection value must be a valid JSON array of strings.");
                }
            }

            throw new FeatureKeyValidationException(
                $"StringCollection value must be a JSON array or JSON string containing an array. Got JSON '{jsonElement.ValueKind}'.");
        }

        if (value is string jsonString)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonString);
                ValidateJsonElement(doc.RootElement, regex, validationRegex);
                return;
            }
            catch (JsonException)
            {
                throw new FeatureKeyValidationException(
                    "StringCollection value must be a valid JSON array of strings.");
            }
        }

        throw new FeatureKeyValidationException(
            $"StringCollection value must be an array of strings. Got '{value.GetType().Name}'.");
    }

    private static void ValidateJsonElement(JsonElement jsonElement, Regex? regex, string? validationRegex)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array)
            throw new FeatureKeyValidationException(
                $"StringCollection value must be an array. Got JSON '{jsonElement.ValueKind}'.");

        var items = new List<string>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String)
                throw new FeatureKeyValidationException(
                    $"StringCollection elements must be strings. Got JSON '{item.ValueKind}'.");

            items.Add(item.GetString()!);
        }

        ValidateItems(items, regex, validationRegex);
    }

    private static Regex? CompileRegex(string? validationRegex)
    {
        if (string.IsNullOrWhiteSpace(validationRegex))
            return null;

        try
        {
            return new Regex(validationRegex, RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        }
        catch (ArgumentException ex)
        {
            throw new FeatureKeyValidationException(
                $"Invalid validation regex pattern '{validationRegex}': {ex.Message}");
        }
    }

    private static void ValidateItems(IEnumerable<string> items, Regex? regex, string? pattern)
    {
        if (regex is null)
            return;

        var index = 0;
        foreach (var item in items)
        {
            if (!regex.IsMatch(item))
                throw new FeatureKeyValidationException(
                    $"StringCollection item at index {index} ('{item}') does not match the validation pattern '{pattern}'.");
            index++;
        }
    }
}
