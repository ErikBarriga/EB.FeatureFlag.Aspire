namespace EB.FeatureFlag.Aspire.Web.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string PrimaryAccessKey { get; set; } = string.Empty;
    public string SecondaryAccessKey { get; set; } = string.Empty;
}

public class EnvironmentModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string PrimaryAccessKey { get; set; } = string.Empty;
    public string SecondaryAccessKey { get; set; } = string.Empty;
}

public class SectionModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

public class FeatureKeyModel
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public int Type { get; set; }
    public object? Value { get; set; }
    public string? ValidationRegex { get; set; }
}

public enum FeatureKeyType
{
    Boolean = 0,
    LargeString = 1,
    StringCollection = 2,
    JsonCollection = 3
}
