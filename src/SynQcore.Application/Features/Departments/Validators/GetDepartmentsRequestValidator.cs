using FluentValidation;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Validators;

public class GetDepartmentsRequestValidator : AbstractValidator<GetDepartmentsRequest>
{
    public GetDepartmentsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Name filter must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Code)
            .MaximumLength(100)
            .WithMessage("Code filter must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.ParentId)
            .NotEmpty()
            .WithMessage("Parent ID must be a valid GUID.")
            .When(x => x.ParentId.HasValue);
    }
}
