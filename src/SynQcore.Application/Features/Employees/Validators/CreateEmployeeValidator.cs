using FluentValidation;
using MediatR;
using SynQcore.Application.Features.Employees.Commands;

namespace SynQcore.Application.Features.Employees.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Request.Phone)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone format")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.Request.Phone));

        RuleFor(x => x.Request.HireDate)
            .NotEmpty().WithMessage("Hire date is required")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Hire date cannot be in the future")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Invalid hire date");

        RuleFor(x => x.Request.DepartmentIds)
            .Must(x => x.Count > 0).WithMessage("Employee must be assigned to at least one department");

        RuleFor(x => x.Request.ManagerId)
            .NotEqual(Guid.Empty).WithMessage("Invalid manager ID")
            .When(x => x.Request.ManagerId.HasValue);
    }
}
