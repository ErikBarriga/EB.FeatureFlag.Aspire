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
            return;
        }

        throw new FeatureKeyValidationException(
            $"StringCollection value must be an array of strings. Got '{value.GetType().Name}'.");
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
