using System.Security.Claims;

namespace EB.FeatureFlag.Auth.Abstractions;

public interface IAuthProvider
{
    string ProviderName { get; }
    AuthUserInfo ExtractUserInfo(IEnumerable<Claim> claims);
}
