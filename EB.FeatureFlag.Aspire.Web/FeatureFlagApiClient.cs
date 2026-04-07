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

    public async Task<ProductModel?> RotateProductKeysAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsync($"/api/products/{id}/rotate-keys", null, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductModel>(ct);
    }

    // Environments
    public async Task<List<EnvironmentModel>> GetEnvironmentsAsync(Guid productId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<EnvironmentModel>>($"/api/products/{productId}/environments", ct) ?? [];

    public async Task<EnvironmentModel?> GetEnvironmentAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<EnvironmentModel>($"/api/environments/{id}", ct);

    public async Task<EnvironmentModel?> CreateEnvironmentAsync(Guid productId, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/products/{productId}/environments", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentModel>(ct);
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

    public async Task<EnvironmentModel?> RotateEnvironmentKeysAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsync($"/api/environments/{id}/rotate-keys", null, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentModel>(ct);
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

    // Feature Keys
    public async Task<List<FeatureKeyModel>> GetFeatureKeysAsync(Guid sectionId, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<FeatureKeyModel>>($"/api/sections/{sectionId}/feature-keys", ct) ?? [];

    public async Task<FeatureKeyModel?> GetFeatureKeyAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<FeatureKeyModel>($"/api/feature-keys/{id}", ct);

    public async Task<FeatureKeyModel?> CreateFeatureKeyAsync(Guid sectionId, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/sections/{sectionId}/feature-keys", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FeatureKeyModel>(ct);
    }

    public async Task<FeatureKeyModel?> UpdateFeatureKeyAsync(Guid id, object request, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/feature-keys/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FeatureKeyModel>(ct);
    }

    public async Task DeleteFeatureKeyAsync(Guid id, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"/api/feature-keys/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
