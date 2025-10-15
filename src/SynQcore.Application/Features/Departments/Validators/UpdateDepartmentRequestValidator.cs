using FluentValidation;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Validators;

public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(200)
            .WithMessage("Department name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Department code is required.")
            .MaximumLength(50)
            .WithMessage("Department code must not exceed 50 characters.")
            .Matches("^[A-Z0-9_-]+$")
            .WithMessage("Department code must contain only uppercase letters, numbers, hyphens, and underscores.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentId)
            .NotEmpty()
            .WithMessage("Parent department ID must be a valid GUID.")
            .When(x => x.ParentId.HasValue);
    }
}
