using Identity.Domain.Entities.Identity;

namespace Identity.Domain.Ports.Security;

public interface IEmailVerificationService
{
    Task<DateTime> SendVerificationAsync(User user, CancellationToken cancellationToken = default);
}
