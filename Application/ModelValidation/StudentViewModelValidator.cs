using FluentValidation;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.ModelValidation
{
    // do lúc đầu lỡ viewmodel là StudentCreateModel nên test ko đúng dc
    public class StudentViewModelValidator : AbstractValidator<StudentViewModel>
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentViewModelValidator(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Frist name is required.")
                .Length(2, 50)
                .WithMessage("Frist name must be between 2 and 50 characters long.")
                .Must(DoesNotExist)
                .WithMessage("Frist name already exists.");

            RuleFor(x => x.Age)
                .InclusiveBetween(1, 3)
                .WithMessage("Age must be between 1 and 100.");

            // RuleForEach dùng để duyệt các collection
            RuleForEach(x => x.Emails)
                .EmailAddress()
                .WithMessage("Invalid email address.");

            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator())
                .WithMessage("Invalid address.");


        }

        private bool DoesNotExist(string fristName)
        {
            return !_dbContext.Student.Any(x => x.FirstName == fristName);
        } 

    }
}
