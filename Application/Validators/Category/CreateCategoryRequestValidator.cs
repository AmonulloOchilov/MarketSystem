using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Category;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(15)
            .WithMessage("Name cannot exceed 15 characters");
    }
}