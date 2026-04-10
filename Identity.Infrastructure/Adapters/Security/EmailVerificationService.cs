using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Adapters.Security;

public class EmailVerificationService : IEmailVerificationService
{
    private readonly IVerificationTokenRepository _verificationTokenRepository;
    private readonly ITokenHashingService _tokenHashingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SmtpEmailSettings _smtpSettings;
    private readonly EmailVerificationSettings _verificationSettings;
    private readonly ILogger<EmailVerificationService> _logger;

    public EmailVerificationService(
        IVerificationTokenRepository verificationTokenRepository,
        ITokenHashingService tokenHashingService,
        IUnitOfWork unitOfWork,
        IOptions<SmtpEmailSettings> smtpOptions,
        IOptions<EmailVerificationSettings> verificationOptions,
        ILogger<EmailVerificationService> logger)
    {
        _verificationTokenRepository = verificationTokenRepository;
        _tokenHashingService = tokenHashingService;
        _unitOfWork = unitOfWork;
        _smtpSettings = smtpOptions.Value;
        _verificationSettings = verificationOptions.Value;
        _logger = logger;
    }

    public async Task<DateTime> SendVerificationAsync(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_verificationSettings.VerificationUrl))
            throw new InvalidOperationException("EmailVerificationSettings:VerificationUrl is required.");

        var rawToken = GenerateToken();
        var expiresAt = DateTime.UtcNow.AddHours(_verificationSettings.TokenExpirationHours);

        await _verificationTokenRepository.InvalidateActiveTokensAsync(
            user.Id,
            VerificationTokenType.EmailVerification,
            cancellationToken);

        await _verificationTokenRepository.AddAsync(
            new VerificationToken
            {
                UserId = user.Id,
                Type = VerificationTokenType.EmailVerification,
                TokenHash = _tokenHashingService.Hash(rawToken),
                ExpiresAt = expiresAt
            },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var verificationLink = BuildVerificationLink(rawToken);
        var recipientName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName ?? user.PrimaryEmail : user.DisplayName;
        var subject = "Verifica tu correo";
        var htmlBody =
            $"""
            <p>Hola {WebUtility.HtmlEncode(recipientName)},</p>
            <p>Gracias por registrarte. Para activar tu cuenta, verifica tu correo haciendo clic en el siguiente enlace:</p>
            <p><a href="{WebUtility.HtmlEncode(verificationLink)}">Verificar correo</a></p>
            <p>Si el boton no funciona, copia y pega esta URL en tu navegador:</p>
            <p>{WebUtility.HtmlEncode(verificationLink)}</p>
            <p>Este enlace expirara el {expiresAt:yyyy-MM-dd HH:mm:ss} UTC.</p>
            """;

        if (!_smtpSettings.Enabled)
        {
            _logger.LogInformation(
                "SMTP disabled. Verification link for {Email}: {VerificationLink}",
                user.PrimaryEmail,
                verificationLink);

            return expiresAt;
        }

        if (string.IsNullOrWhiteSpace(_smtpSettings.Host))
            throw new InvalidOperationException("SmtpEmailSettings:Host is required when SMTP is enabled.");

        if (string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
            throw new InvalidOperationException("SmtpEmailSettings:FromEmail is required when SMTP is enabled.");

        using var message = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(user.PrimaryEmail);

        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            EnableSsl = _smtpSettings.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_smtpSettings.UserName))
        {
            client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
        return expiresAt;
    }

    private string BuildVerificationLink(string rawToken)
    {
        var separator = _verificationSettings.VerificationUrl.Contains('?') ? '&' : '?';
        return $"{_verificationSettings.VerificationUrl}{separator}token={Uri.EscapeDataString(rawToken)}";
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
