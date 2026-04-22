using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Employee;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Firstname is required").MaximumLength(20).WithMessage("Firstname cannot exceed 20 characters");
        
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Lastname is required").MaximumLength(20).WithMessage("Lastname cannot exceed 20 characters");

        RuleFor(x => x.Position).NotEmpty().WithMessage("Position is required").MaximumLength(15)
            .WithMessage("Position cannot exceed 15 characters");
    }
}