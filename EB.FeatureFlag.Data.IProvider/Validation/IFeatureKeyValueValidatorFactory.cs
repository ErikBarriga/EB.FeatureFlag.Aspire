using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.IProvider.Validation;

/// <summary>
/// Dispatches validation to the correct <see cref="IFeatureKeyValueValidator"/> based on type.
/// </summary>
public interface IFeatureKeyValueValidatorFactory
{
    void Validate(FeatureKeyType type, object? value, string? validationRegex = null);
}
