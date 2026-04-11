using EB.FeatureFlag.Aspire.Web;
using EB.FeatureFlag.Aspire.Web.Components;
using EB.FeatureFlag.Auth;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

// Authentication
builder.Services.AddFeatureFlagWebAuth(options =>
{
    var providerType = builder.Configuration["Auth_ProviderType"] ?? "Google";
    options.ProviderType = Enum.Parse<FeatureFlagAuthProviderType>(providerType, ignoreCase: true);
    options.ClientId = builder.Configuration["Auth_Google_ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Auth_Google_ClientSecret"] ?? "";
    options.JwtSigningKey = builder.Configuration["Auth_JwtSigningKey"] ?? "FeatureFlag_Default_Dev_Key_Change_In_Prod_32chars!";
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddHttpClient<FeatureFlagApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

// Login/Logout endpoints
app.MapGet("/login", (HttpContext context, string? returnUrl) =>
{
    return Results.Challenge(new AuthenticationProperties
    {
        RedirectUri = returnUrl ?? "/"
    });
}).AllowAnonymous();

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Cookies");
    return Results.Redirect("/");
}).AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.MapDefaultEndpoints();

app.Run();
