namespace EB.FeatureFlag.Aspire.ApiService.Models;

public record CreateEnvironmentRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);

public record UpdateEnvironmentRequest(
    string Name,
    string? Description = null,
    List<string>? Tags = null);

public record RotateKeyRequest(string KeyType);

// Response models — access keys are secrets and must not leak to the UI
public record EnvironmentResponse(
    Guid Id,
    Guid ProductId,
    string Name,
    string? Description,
    List<string>? Tags);

public record EnvironmentCreatedResponse(
    Guid Id,
    Guid ProductId,
    string Name,
    string? Description,
    List<string>? Tags,
    string PrimaryAccessKey,
    string SecondaryAccessKey);

public record EnvironmentRotatedKeyResponse(
    Guid Id,
    string KeyType,
    string NewKey);
