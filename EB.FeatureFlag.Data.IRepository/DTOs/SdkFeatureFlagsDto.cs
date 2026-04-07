namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class SdkSectionFlagsDto
{
    public string SectionName { get; set; } = default!;
    public List<FeatureKeyDto> FeatureKeys { get; set; } = [];
}
