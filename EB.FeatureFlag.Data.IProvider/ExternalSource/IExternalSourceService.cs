using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.IProvider.ExternalSource;

public interface IExternalSourceService
{
    Task<object?> FetchExternalValueAsync(ExternalSourceConfigDto config, FeatureKeyType type, CancellationToken cancellationToken = default);
}
