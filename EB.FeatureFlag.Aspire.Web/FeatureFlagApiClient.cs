using EB.FeatureFlag.Aspire.Web.Models;

namespace EB.FeatureFlag.Aspire.Web;

public class FeatureFlagApiClient(HttpClient httpClient)
{
    // Products
    public async Task<List<ProductModel>> GetProductsAsync(CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<ProductModel>>("/api/products", ct) ?? [];

    public async Task<ProductModel?> GetProductAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<ProductModel>($"/api/products/{id}", ct);

    public async Task<ProductModel?> CreateProductAsync(object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductModel>(ct);
    }

    public async Task<ProductModel?> UpdateProductAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/products/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductModel>(ct);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/products/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // Environments
    public async Task<List<EnvironmentModel>> GetEnvironmentsAsync(Guid productId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<EnvironmentModel>>($"/api/products/{productId}/environments", ct) ?? [];

    public async Task<EnvironmentModel?> GetEnvironmentAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<EnvironmentModel>($"/api/environments/{id}", ct);

    public async Task<EnvironmentCreatedModel?> CreateEnvironmentAsync(Guid productId, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/products/{productId}/environments", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentCreatedModel>(ct);
    }

    public async Task<EnvironmentModel?> UpdateEnvironmentAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/environments/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentModel>(ct);
    }

    public async Task DeleteEnvironmentAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/environments/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<EnvironmentRotatedKeyModel?> RotateEnvironmentKeysAsync(Guid id, string keyType, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/environments/{id}/rotate-keys", new { KeyType = keyType }, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentRotatedKeyModel>(ct);
    }

    // Sections
    public async Task<List<SectionModel>> GetSectionsByProductAsync(Guid productId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<SectionModel>>($"/api/products/{productId}/sections", ct) ?? [];

    public async Task<SectionModel?> GetSectionAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<SectionModel>($"/api/sections/{id}", ct);

    public async Task<SectionModel?> CreateSectionAsync(Guid productId, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/products/{productId}/sections", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SectionModel>(ct);
    }

    public async Task<SectionModel?> UpdateSectionAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/sections/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SectionModel>(ct);
    }

    public async Task DeleteSectionAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/sections/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // Feature Flags
    public async Task<List<FeatureFlagModel>> GetFeatureFlagsByProductAsync(Guid productId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<FeatureFlagModel>>($"/api/products/{productId}/feature-flags", ct) ?? [];

    public async Task<List<FeatureFlagModel>> GetFeatureFlagsBySectionAsync(Guid sectionId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<FeatureFlagModel>>($"/api/sections/{sectionId}/feature-flags", ct) ?? [];

    public async Task<FeatureFlagModel?> GetFeatureFlagAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<FeatureFlagModel>($"/api/feature-flags/{id}", ct);

    public async Task<FeatureFlagModel?> CreateFeatureFlagAsync(Guid sectionId, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/sections/{sectionId}/feature-flags", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FeatureFlagModel>(ct);
    }

    public async Task<FeatureFlagModel?> UpdateFeatureFlagAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/feature-flags/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FeatureFlagModel>(ct);
    }

    public async Task DeleteFeatureFlagAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/feature-flags/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // Feature Flag Details (per-environment values)
    public async Task<List<FeatureFlagDetailModel>> GetFeatureFlagDetailsAsync(Guid featureFlagId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<FeatureFlagDetailModel>>($"/api/feature-flags/{featureFlagId}/details", ct) ?? [];

    public async Task<FeatureFlagDetailModel?> GetFeatureFlagDetailAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<FeatureFlagDetailModel>($"/api/feature-flag-details/{id}", ct);

    public async Task<FeatureFlagDetailModel?> UpdateFeatureFlagDetailAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/feature-flag-details/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FeatureFlagDetailModel>(ct);
    }

    // SDK Explorer
    public async Task<SdkSingleFeatureFlagResponseModel?> GetSdkFeatureFlagAsync(string environmentKey, string flagName, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/sdk/feature-flags/{Uri.EscapeDataString(flagName)}");
        request.Headers.Add("X-Environment-Key", environmentKey);
        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SdkSingleFeatureFlagResponseModel>(ct);
    }
}
