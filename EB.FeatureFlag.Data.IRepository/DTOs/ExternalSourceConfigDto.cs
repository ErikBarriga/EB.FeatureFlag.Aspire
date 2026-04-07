namespace EB.FeatureFlag.Data.IRepository.DTOs;

public class ExternalSourceAuthDto
{
    public string? Scheme { get; set; }
    public string? Parameter { get; set; }
}

public class ExternalSourceConfigDto
{
    public string Url { get; set; } = default!;
    public string Method { get; set; } = "GET";
    public Dictionary<string, string>? Headers { get; set; }
    public ExternalSourceAuthDto? Auth { get; set; }
    public string? MappingPath { get; set; }
    public string? Body { get; set; }
    public string? ContentType { get; set; }
}
