using FluentValidation;

namespace Identity.Application.UseCase.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.UserAgent)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.UserAgent));

        RuleFor(x => x.DeviceName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceName));

        RuleFor(x => x.IpAddress)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.IpAddress));
    }
}