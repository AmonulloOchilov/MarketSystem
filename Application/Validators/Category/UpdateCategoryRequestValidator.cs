using Application.DTOs.Request;
using FluentValidation;

namespace Application.Validators.Category;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(15)
            .WithMessage("Name cannot exceed 15 characters");
    }
}