using FluentValidation;

namespace Application.Users.Commands.RegisterUser
{
	public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
	{
		public RegisterUserCommandValidator()
		{
			RuleFor(c => c.Email).NotEmpty();
			RuleFor(c => c.Username).NotEmpty();
			RuleFor(c => c.Password).NotEmpty();
		}
	}
}
