namespace Identity.Domain.Ports.Security;

public interface ITokenHashingService
{
    string Hash(string value);
}