using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Privacy.DTOs;

namespace SynQcore.Application.Features.Privacy.Queries;

/// <summary>
/// Query para obter consentimentos com paginação e filtros
/// </summary>
public class GetConsentRecordsQuery : IRequest<PagedResult<ConsentRecordDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public Guid? EmployeeId { get; set; }
    public string? ConsentCategory { get; set; }
    public bool? ConsentGranted { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
    public string? OrderBy { get; set; } = "ConsentDate";
    public bool OrderDescending { get; set; } = true;
}

/// <summary>
/// Query para obter consentimento específico por ID
/// </summary>
public class GetConsentRecordByIdQuery : IRequest<ConsentRecordDto?>
{
    public Guid Id { get; set; }

    public GetConsentRecordByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Query para obter consentimentos de um funcionário específico
/// </summary>
public class GetEmployeeConsentsQuery : IRequest<List<ConsentRecordDto>>
{
    public Guid EmployeeId { get; set; }
    public bool? IsActive { get; set; }

    public GetEmployeeConsentsQuery(Guid employeeId, bool? isActive = null)
    {
        EmployeeId = employeeId;
        IsActive = isActive;
    }
}

/// <summary>
/// Query para obter solicitações de exportação com paginação
/// </summary>
public class GetDataExportRequestsQuery : IRequest<PagedResult<DataExportRequestDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public Guid? EmployeeId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Format { get; set; }
    public string? OrderBy { get; set; } = "RequestDate";
    public bool OrderDescending { get; set; } = true;
}

/// <summary>
/// Query para obter solicitação de exportação específica
/// </summary>
public class GetDataExportRequestByIdQuery : IRequest<DataExportRequestDto?>
{
    public Guid Id { get; set; }

    public GetDataExportRequestByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Query para obter exportações de um funcionário específico
/// </summary>
public class GetEmployeeExportRequestsQuery : IRequest<List<DataExportRequestDto>>
{
    public Guid EmployeeId { get; set; }
    public string? Status { get; set; }

    public GetEmployeeExportRequestsQuery(Guid employeeId, string? status = null)
    {
        EmployeeId = employeeId;
        Status = status;
    }
}

/// <summary>
/// Query para obter solicitações de exclusão com paginação
/// </summary>
public class GetDataDeletionRequestsQuery : IRequest<PagedResult<DataDeletionRequestDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public Guid? EmployeeId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? DeletionType { get; set; }
    public string? OrderBy { get; set; } = "RequestDate";
    public bool OrderDescending { get; set; } = true;
}

/// <summary>
/// Query para obter solicitação de exclusão específica
/// </summary>
public class GetDataDeletionRequestByIdQuery : IRequest<DataDeletionRequestDto?>
{
    public Guid Id { get; set; }

    public GetDataDeletionRequestByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Query para obter exclusões de um funcionário específico
/// </summary>
public class GetEmployeeDeletionRequestsQuery : IRequest<List<DataDeletionRequestDto>>
{
    public Guid EmployeeId { get; set; }
    public string? Status { get; set; }

    public GetEmployeeDeletionRequestsQuery(Guid employeeId, string? status = null)
    {
        EmployeeId = employeeId;
        Status = status;
    }
}

/// <summary>
/// Query para obter categorias de dados pessoais
/// </summary>
public class GetPersonalDataCategoriesQuery : IRequest<PagedResult<PersonalDataCategoryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public bool? IsActive { get; set; }
    public string? SensitivityLevel { get; set; }
    public bool? RequiresConsent { get; set; }
    public string? OrderBy { get; set; } = "CategoryName";
    public bool OrderDescending { get; set; }
}

/// <summary>
/// Query para obter categoria específica de dados pessoais
/// </summary>
public class GetPersonalDataCategoryByIdQuery : IRequest<PersonalDataCategoryDto?>
{
    public Guid Id { get; set; }

    public GetPersonalDataCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Query para obter relatório de compliance
/// </summary>
public class GetComplianceReportQuery : IRequest<ComplianceReportDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? ConsentCategory { get; set; }

    public GetComplianceReportQuery(DateTime? startDate = null, DateTime? endDate = null)
    {
        StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
        EndDate = endDate ?? DateTime.UtcNow;
    }
}

/// <summary>
/// Query para obter dashboard de privacidade
/// </summary>
public class GetPrivacyDashboardQuery : IRequest<PrivacyDashboardDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public GetPrivacyDashboardQuery(DateTime? startDate = null, DateTime? endDate = null)
    {
        StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
        EndDate = endDate ?? DateTime.UtcNow;
    }
}

/// <summary>
/// Query para verificar expiração de consentimentos
/// </summary>
public class GetExpiringConsentsQuery : IRequest<List<ConsentRecordDto>>
{
    public int DaysInAdvance { get; set; } = 30;

    public GetExpiringConsentsQuery(int daysInAdvance = 30)
    {
        DaysInAdvance = daysInAdvance;
    }
}

/// <summary>
/// Query para verificar solicitações pendentes
/// </summary>
public class GetPendingRequestsQuery : IRequest<PrivacyDashboardDto>
{
    public DateTime? DeadlineDate { get; set; }

    public GetPendingRequestsQuery(DateTime? deadlineDate = null)
    {
        DeadlineDate = deadlineDate ?? DateTime.UtcNow.AddDays(-7);
    }
}
