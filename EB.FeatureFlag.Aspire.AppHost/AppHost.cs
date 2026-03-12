var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EB_FeatureFlag_Aspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.EB_FeatureFlag_Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
