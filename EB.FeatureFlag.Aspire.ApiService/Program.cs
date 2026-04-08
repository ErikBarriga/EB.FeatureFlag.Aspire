using EB.FeatureFlag.Aspire.ApiService.Endpoints;
using EB.FeatureFlag.Data;
using EB.FeatureFlag.Data.Repository.SQLite.Context;

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

// Apply SQLite migrations if using SQLite
var repoType = app.Configuration["FeatureFlag_RepositoryType"] ?? "Cosmos";
if (Enum.Parse<FeatureFlagRepositoryType>(repoType, ignoreCase: true) == FeatureFlagRepositoryType.SQLite)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FeatureFlagSqliteDbContext>();
    dbContext.Database.EnsureCreated();
}

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
app.MapFeatureFlagEndpoints();
app.MapSdkEndpoints();

app.MapDefaultEndpoints();

app.Run();
