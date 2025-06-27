using FluentValidation;
using TodoWeb.Application.Dtos.StudentDTO;

namespace TodoWeb.Application.ModelValidation
{
    public class AddressValidator: AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Street is required.");
            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("ZipCode is required.");
        
        }
    }
}
