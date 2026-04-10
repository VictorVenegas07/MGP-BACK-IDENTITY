using Identity.Domain.Entities.Auth;

namespace Identity.Domain.Ports.Security;

public interface ITokenService
{
    string GenerateAccessToken(JwtPayloadData payload);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpirationUtc();
    DateTime GetRefreshTokenExpirationUtc();
}