using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EB.FeatureFlag.Data.IProvider.ExternalSource;
using EB.FeatureFlag.Data.IRepository.DTOs;
using EB.FeatureFlag.Data.IRepository.Types;

namespace EB.FeatureFlag.Data.Provider.ExternalSource;

public class ExternalSourceService : IExternalSourceService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalSourceService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<object?> FetchExternalValueAsync(ExternalSourceConfigDto config, FeatureKeyType type, CancellationToken cancellationToken = default)
    {
        if (config == null || string.IsNullOrWhiteSpace(config.Url))
            return null;

        using var request = new HttpRequestMessage(new HttpMethod(config.Method ?? "GET"), config.Url);

        if (config.Headers != null)
        {
            foreach (var header in config.Headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (config.Auth != null)
        {
            if (!string.IsNullOrWhiteSpace(config.Auth.Scheme) && !string.IsNullOrWhiteSpace(config.Auth.Parameter))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(config.Auth.Scheme, config.Auth.Parameter);
            }
            else if (!string.IsNullOrWhiteSpace(config.Auth.Parameter))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.Auth.Parameter);
            }
        }

        if (!string.Equals(config.Method, "GET", StringComparison.OrdinalIgnoreCase) && config.Body != null)
        {
            request.Content = new StringContent(config.Body, Encoding.UTF8, config.ContentType ?? "application/json");
        }

        var client = _httpClientFactory.CreateClient("FeatureFlagExternalSource");
        client.Timeout = TimeSpan.FromSeconds(5);

        using var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
            return null;

        using var document = JsonDocument.Parse(content);
        var target = TrySelectToken(document.RootElement, config.MappingPath);
        return ConvertJsonElement(target, type);
    }

    private static JsonElement TrySelectToken(JsonElement element, string? mappingPath)
    {
        if (string.IsNullOrWhiteSpace(mappingPath))
            return element;

        var segments = mappingPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var current = element;

        foreach (var segment in segments)
        {
            if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(segment, out var next))
            {
                current = next;
                continue;
            }

            if (current.ValueKind == JsonValueKind.Array && int.TryParse(segment, out var index) && index >= 0 && index < current.GetArrayLength())
            {
                current = current[index];
                continue;
            }

            return default;
        }

        return current;
    }

    private static object? ConvertJsonElement(JsonElement element, FeatureKeyType type)
    {
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            return null;

        return type switch
        {
            FeatureKeyType.Boolean => element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False
                ? element.GetBoolean()
                : bool.TryParse(element.GetString(), out var boolValue) ? boolValue : null,
            FeatureKeyType.LargeString => element.ValueKind == JsonValueKind.String
                ? element.GetString()
                : element.GetRawText(),
            FeatureKeyType.StringCollection => element.ValueKind == JsonValueKind.Array
                ? element.EnumerateArray().Select(item => item.ValueKind == JsonValueKind.String ? item.GetString() ?? string.Empty : item.GetRawText()).ToList()
                : element.ValueKind == JsonValueKind.String ? new List<string> { element.GetString() ?? string.Empty } : null,
            FeatureKeyType.JsonCollection => element.ValueKind == JsonValueKind.Array
                ? JsonSerializer.Deserialize<List<object>>(element.GetRawText())
                : JsonSerializer.Deserialize<object>(element.GetRawText()),
            _ => JsonSerializer.Deserialize<object>(element.GetRawText()),
        };
    }
}
