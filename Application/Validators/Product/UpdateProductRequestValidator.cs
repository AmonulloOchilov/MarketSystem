using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Product;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required").MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater then zero");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater then 0");
    }
}