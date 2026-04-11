using System.Security.Claims;
using EB.FeatureFlag.Auth.Abstractions;

namespace EB.FeatureFlag.Auth.Google;

public class GoogleAuthProvider : IAuthProvider
{
    public string ProviderName => "Google";

    public AuthUserInfo ExtractUserInfo(IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();

        var externalId = claimList.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                      ?? claimList.FirstOrDefault(c => c.Type == "sub")?.Value
                      ?? throw new InvalidOperationException("Google claims missing subject identifier.");

        var email = claimList.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                 ?? throw new InvalidOperationException("Google claims missing email.");

        var displayName = claimList.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? email;

        var pictureUrl = claimList.FirstOrDefault(c => c.Type == "picture")?.Value;

        return new AuthUserInfo
        {
            ExternalId = externalId,
            Provider = ProviderName,
            Email = email,
            DisplayName = displayName,
            PictureUrl = pictureUrl
        };
    }
}
