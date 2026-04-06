using System.Text.Json;
using System.Text.RegularExpressions;
using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class LargeStringValueValidator : IFeatureKeyValueValidator
{
    public FeatureKeyType SupportedType => FeatureKeyType.LargeString;

    public void Validate(object? value, string? validationRegex = null)
    {
        if (value is null)
            throw new FeatureKeyValidationException("LargeString value cannot be null.");

        string str;

        if (value is string s)
        {
            str = s;
        }
        else if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind != JsonValueKind.String)
                throw new FeatureKeyValidationException(
                    $"LargeString value must be a string. Got JSON '{jsonElement.ValueKind}'.");

            str = jsonElement.GetString()!;
        }
        else
        {
            throw new FeatureKeyValidationException(
                $"LargeString value must be a string. Got '{value.GetType().Name}'.");
        }

        ValidateRegex(str, validationRegex);
    }

    private static void ValidateRegex(string value, string? validationRegex)
    {
        if (string.IsNullOrWhiteSpace(validationRegex))
            return;

        Regex regex;
        try
        {
            regex = new Regex(validationRegex, RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        }
        catch (ArgumentException ex)
        {
            throw new FeatureKeyValidationException(
                $"Invalid validation regex pattern '{validationRegex}': {ex.Message}");
        }

        if (!regex.IsMatch(value))
            throw new FeatureKeyValidationException(
                $"LargeString value does not match the validation pattern '{validationRegex}'.");
    }
}
