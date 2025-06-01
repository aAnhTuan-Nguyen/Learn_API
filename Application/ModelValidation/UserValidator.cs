using FluentValidation;
using TodoWeb.Application.Dtos.UserDTO;

namespace TodoWeb.Application.ModelValidation
{
    public class UserValidator: AbstractValidator<UserCreateModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Role is not valid.");
        }
    }
}
