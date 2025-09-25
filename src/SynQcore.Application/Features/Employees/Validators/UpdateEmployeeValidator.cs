using MediatR;
using FluentValidation;
using SynQcore.Application.Features.Employees.Commands;

namespace SynQcore.Application.Features.Employees.Validators;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Employee ID is required")
            .NotEqual(Guid.Empty).WithMessage("Invalid employee ID");

        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.Request.Phone)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone format")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.Request.Phone));

        RuleFor(x => x.Request.DepartmentIds)
            .Must(x => x.Count > 0).WithMessage("Employee must be assigned to at least one department");

        RuleFor(x => x.Request.ManagerId)
            .NotEqual(Guid.Empty).WithMessage("Invalid manager ID")
            .When(x => x.Request.ManagerId.HasValue);

        // Validação para evitar auto-referência
        RuleFor(x => x)
            .Must(x => x.Request.ManagerId != x.Id)
            .WithMessage("Employee cannot be their own manager")
            .When(x => x.Request.ManagerId.HasValue);
    }
}