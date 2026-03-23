using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Customer;

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Firstname is required").MaximumLength(20).WithMessage("Firstname cannot exceed 20 characters");
        
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Lastname is required").MaximumLength(20).WithMessage("Lastname cannot exceed 20 characters");

        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber).Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format");
    }
}