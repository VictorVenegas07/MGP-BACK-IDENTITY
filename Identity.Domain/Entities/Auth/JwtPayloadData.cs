namespace Identity.Domain.Entities.Auth;

public class JwtPayloadData
{
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }

}
