namespace EB.FeatureFlag.Aspire.Web.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

public class EnvironmentModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

public class EnvironmentCreatedModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string PrimaryAccessKey { get; set; } = string.Empty;
    public string SecondaryAccessKey { get; set; } = string.Empty;
}

public class EnvironmentRotatedKeyModel
{
    public Guid Id { get; set; }
    public string KeyType { get; set; } = string.Empty;
    public string NewKey { get; set; } = string.Empty;
}

public class SectionModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}

public class FeatureFlagModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid SectionId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public int Type { get; set; }
    public string? ValidationRegex { get; set; }
}

public class FeatureFlagDetailModel
{
    public Guid Id { get; set; }
    public Guid FeatureFlagId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid ProductId { get; set; }
    public object? Value { get; set; }
}

public enum FeatureKeyType
{
    Boolean = 0,
    LargeString = 1,
    StringCollection = 2,
    JsonCollection = 3
}

// SDK Explorer models
public class SdkSingleFeatureFlagResponseModel
{
    public string Product { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int Type { get; set; }
    public object? Value { get; set; }
}

public class SdkFeatureFlagValueResponseModel
{
    public int Type { get; set; }
    public object? Value { get; set; }
}
