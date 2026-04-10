using FluentValidation;

namespace Identity.Application.UseCase.Auth.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.UserName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.UserName));

        RuleFor(x => x.DisplayName)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}