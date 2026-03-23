using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Admin;

public class CreateAdminRequestValidator : AbstractValidator<CreateAdminRequest>
{
    public CreateAdminRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Firstname is required").MaximumLength(20).WithMessage("Firstname cannot exceed 20 characters");
        
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Lastname is required").MaximumLength(20).WithMessage("Lastname cannot exceed 20 characters");

        RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required").MaximumLength(25)
            .WithMessage("Role cannot exceed 25 characters");

        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}