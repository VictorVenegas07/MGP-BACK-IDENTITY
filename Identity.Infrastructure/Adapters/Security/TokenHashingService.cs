using System.Security.Cryptography;
using System.Text;
using Identity.Domain.Ports.Security;

namespace Identity.Infrastructure.Adapters.Security;

public class TokenHashingService : ITokenHashingService
{
    public string Hash(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}