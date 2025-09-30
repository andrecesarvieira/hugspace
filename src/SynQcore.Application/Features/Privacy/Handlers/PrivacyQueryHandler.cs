using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Application.Features.Privacy.Queries;
using SynQcore.Application.Features.Privacy.Utilities;

namespace SynQcore.Application.Features.Privacy.Handlers;

/// <summary>
/// Handler principal para queries de privacidade e compliance LGPD
/// </summary>
public partial class PrivacyQueryHandler :
    IRequestHandler<GetConsentRecordsQuery, PagedResult<ConsentRecordDto>>,
    IRequestHandler<GetConsentRecordByIdQuery, ConsentRecordDto?>,
    IRequestHandler<GetPersonalDataCategoriesQuery, PagedResult<PersonalDataCategoryDto>>,
    IRequestHandler<GetPersonalDataCategoryByIdQuery, PersonalDataCategoryDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<PrivacyQueryHandler> _logger;

    public PrivacyQueryHandler(ISynQcoreDbContext context, ILogger<PrivacyQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region LoggerMessage Delegates

    [LoggerMessage(LogLevel.Information, "Processando consulta de consentimentos - Página: {page}, Tamanho: {pageSize}, FuncionarioId: {employeeId}")]
    private static partial void LogProcessingConsentRecords(ILogger logger, int page, int pageSize, Guid? employeeId);

    [LoggerMessage(LogLevel.Information, "Consentimento encontrado - Id: {id}")]
    private static partial void LogConsentRecordFound(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Warning, "Consentimento não encontrado - Id: {id}")]
    private static partial void LogConsentRecordNotFound(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Processando categorias de dados - Página: {page}, Ativo: {isActive}")]
    private static partial void LogProcessingDataCategories(ILogger logger, int page, bool? isActive);

    [LoggerMessage(LogLevel.Error, "Erro ao processar consulta de privacidade - Operação: {operation}")]
    private static partial void LogPrivacyQueryError(ILogger logger, string operation, Exception exception);

    #endregion

    #region Consent Records

    public async Task<PagedResult<ConsentRecordDto>> Handle(GetConsentRecordsQuery request, CancellationToken cancellationToken)
    {
        LogProcessingConsentRecords(_logger, request.Page, request.PageSize, request.EmployeeId);

        try
        {
            var query = _context.ConsentRecords
                .Include(x => x.Employee)
                .AsQueryable();

            // Aplicar filtros
            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

            if (!string.IsNullOrEmpty(request.ConsentCategory))
                query = query.Where(x => x.ConsentCategory.Contains(request.ConsentCategory));

            if (request.ConsentGranted.HasValue)
                query = query.Where(x => x.ConsentGranted == request.ConsentGranted.Value);

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (request.StartDate.HasValue)
                query = query.Where(x => x.ConsentDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(x => x.ConsentDate <= request.EndDate.Value);

            // Aplicar ordenação
            query = request.OrderBy?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "consentdate" => request.OrderDescending
                    ? query.OrderByDescending(x => x.ConsentDate)
                    : query.OrderBy(x => x.ConsentDate),
                "category" => request.OrderDescending
                    ? query.OrderByDescending(x => x.ConsentCategory)
                    : query.OrderBy(x => x.ConsentCategory),
                "employee" => request.OrderDescending
                    ? query.OrderByDescending(x => x.Employee.FirstName)
                    : query.OrderBy(x => x.Employee.FirstName),
                _ => request.OrderDescending
                    ? query.OrderByDescending(x => x.ConsentDate)
                    : query.OrderBy(x => x.ConsentDate)
            };

            return await query.ToPaginatedResultAsync(
                request.Page,
                request.PageSize,
                entity => entity.ToConsentRecordDto(),
                cancellationToken);
        }
        catch (Exception ex)
        {
            LogPrivacyQueryError(_logger, "GetConsentRecords", ex);
            throw;
        }
    }

    public async Task<ConsentRecordDto?> Handle(GetConsentRecordByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.ConsentRecords
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                LogConsentRecordNotFound(_logger, request.Id);
                return null;
            }

            LogConsentRecordFound(_logger, request.Id);
            return entity.ToConsentRecordDto();
        }
        catch (Exception ex)
        {
            LogPrivacyQueryError(_logger, "GetConsentRecordById", ex);
            throw;
        }
    }

    #endregion

    #region Personal Data Categories

    public async Task<PagedResult<PersonalDataCategoryDto>> Handle(GetPersonalDataCategoriesQuery request, CancellationToken cancellationToken)
    {
        LogProcessingDataCategories(_logger, request.Page, request.IsActive);

        try
        {
            var query = _context.PersonalDataCategories
                .AsQueryable();

            // Aplicar filtros
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.SensitivityLevel))
                query = query.Where(x => x.SensitivityLevel.ToString() == request.SensitivityLevel);

            if (request.RequiresConsent.HasValue)
                query = query.Where(x => x.RequiresConsent == request.RequiresConsent.Value);

            // Aplicar ordenação
            query = request.OrderBy?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "categoryname" => request.OrderDescending
                    ? query.OrderByDescending(x => x.CategoryName)
                    : query.OrderBy(x => x.CategoryName),
                "sensitivitylevel" => request.OrderDescending
                    ? query.OrderByDescending(x => x.SensitivityLevel)
                    : query.OrderBy(x => x.SensitivityLevel),
                "effectivedate" => request.OrderDescending
                    ? query.OrderByDescending(x => x.EffectiveDate)
                    : query.OrderBy(x => x.EffectiveDate),
                _ => request.OrderDescending
                    ? query.OrderByDescending(x => x.CategoryName)
                    : query.OrderBy(x => x.CategoryName)
            };

            return await query.ToPaginatedResultAsync(
                request.Page,
                request.PageSize,
                entity => entity.ToPersonalDataCategoryDto(),
                cancellationToken);
        }
        catch (Exception ex)
        {
            LogPrivacyQueryError(_logger, "GetPersonalDataCategories", ex);
            throw;
        }
    }

    public async Task<PersonalDataCategoryDto?> Handle(GetPersonalDataCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.PersonalDataCategories
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return entity?.ToPersonalDataCategoryDto();
        }
        catch (Exception ex)
        {
            LogPrivacyQueryError(_logger, "GetPersonalDataCategoryById", ex);
            throw;
        }
    }

    #endregion
}
