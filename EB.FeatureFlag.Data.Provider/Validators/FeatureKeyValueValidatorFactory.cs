using EB.FeatureFlag.Data.IProvider.Validation;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.Validators;

public class FeatureKeyValueValidatorFactory(IEnumerable<IFeatureKeyValueValidator> validators)
    : IFeatureKeyValueValidatorFactory
{
    private readonly Dictionary<FeatureKeyType, IFeatureKeyValueValidator> _validators =
        validators.ToDictionary(v => v.SupportedType);

    public void Validate(FeatureKeyType type, object? value, string? validationRegex = null)
    {
        if (!_validators.TryGetValue(type, out var validator))
            throw new FeatureKeyValidationException($"No validator registered for type '{type}'.");

        validator.Validate(value, validationRegex);
    }
}
