using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EB.FeatureFlag.Auth.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace EB.FeatureFlag.Auth.Services;

public class JwtTokenService
{
    private readonly FeatureFlagAuthOptions _options;

    public JwtTokenService(FeatureFlagAuthOptions options)
    {
        _options = options;
    }

    public string GenerateToken(IAuthUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("name", user.DisplayName),
            new("provider", user.Provider)
        };

        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

        var token = new JwtSecurityToken(
            issuer: _options.JwtIssuer,
            audience: _options.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.JwtTokenLifetimeMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
