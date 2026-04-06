using EB.FeatureFlag.Aspire.ApiService.Endpoints;
using EB.FeatureFlag.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Feature Flag data layer (repository + cache + provider)
builder.Services.AddFeatureFlagData(options =>
{
    var repoType = builder.Configuration["FeatureFlag_RepositoryType"] ?? "Cosmos";
    options.RepositoryType = Enum.Parse<FeatureFlagRepositoryType>(repoType, ignoreCase: true);
    options.RepositoryConnectionString = builder.Configuration["FeatureFlag_RepositoryConnectionString"] ?? string.Empty;

    var cacheType = builder.Configuration["FeatureFlag_CacheType"] ?? "None";
    options.CacheType = Enum.Parse<FeatureFlagCacheType>(cacheType, ignoreCase: true);
    options.CacheConnectionString = builder.Configuration["FeatureFlag_CacheConnectionString"] ?? string.Empty;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map Feature Flag endpoints
app.MapProductEndpoints();
app.MapEnvironmentEndpoints();
app.MapSectionEndpoints();
app.MapFeatureKeyEndpoints();

app.MapDefaultEndpoints();

app.Run();
