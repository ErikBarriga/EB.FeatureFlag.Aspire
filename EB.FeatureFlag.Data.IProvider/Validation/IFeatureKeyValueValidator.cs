using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.IProvider.Validation;

/// <summary>
/// Validates the value of a Feature Key based on its type.
/// Implement this interface to add support for new Feature Key types.
/// </summary>
public interface IFeatureKeyValueValidator
{
    FeatureKeyType SupportedType { get; }
    void Validate(object? value, string? validationRegex = null);
}
