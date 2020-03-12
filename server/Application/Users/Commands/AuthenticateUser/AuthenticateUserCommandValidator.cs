using FluentValidation;

namespace Application.Users.Commands.AuthenticateUser
{
	public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
	{
		public AuthenticateUserCommandValidator()
		{
			RuleFor(c => c.Email).NotEmpty();
			RuleFor(c => c.Password).NotEmpty();
		}
	}
}
