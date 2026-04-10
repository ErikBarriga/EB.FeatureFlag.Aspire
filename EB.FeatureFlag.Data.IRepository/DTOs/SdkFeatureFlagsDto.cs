using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class SdkSectionFlagsDto
{
    public string SectionName { get; set; } = default!;
    public List<SdkFeatureFlagItemDto> FeatureFlags { get; set; } = [];
}

public class SdkFeatureFlagItemDto
{
    public string Key { get; set; } = default!;
    public FeatureKeyType Type { get; set; }
    public object? Value { get; set; }
}

public class SdkFeatureFlagValueDto
{
    public FeatureKeyType Type { get; set; }
    public object? Value { get; set; }
}
