using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Privacy.Commands;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Application.Features.Privacy.Utilities;
using SynQcore.Domain.Entities;
using static SynQcore.Domain.Entities.RequestStatus;

namespace SynQcore.Application.Features.Privacy.Handlers;

/// <summary>
/// Handler para comandos Create de Privacy
/// </summary>
public partial class PrivacyCreateHandlers :
    IRequestHandler<CreateConsentRecordCommand, ConsentRecordDto>,
    IRequestHandler<CreateDataExportRequestCommand, DataExportRequestDto>,
    IRequestHandler<CreateDataDeletionRequestCommand, DataDeletionRequestDto>,
    IRequestHandler<CreatePersonalDataCategoryCommand, PersonalDataCategoryDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<PrivacyCreateHandlers> _logger;

    public PrivacyCreateHandlers(
        ISynQcoreDbContext context,
        ILogger<PrivacyCreateHandlers> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region LoggerMessage Delegates

    [LoggerMessage(LogLevel.Information, "Criando registro de consentimento - Funcionário: {employeeId}, Categoria: {category}")]
    private static partial void LogCreatingConsentRecord(ILogger logger, Guid employeeId, string category);

    [LoggerMessage(LogLevel.Information, "Criando solicitação de exportação - Funcionário: {employeeId}")]
    private static partial void LogCreatingExportRequest(ILogger logger, Guid employeeId);

    [LoggerMessage(LogLevel.Information, "Criando solicitação de exclusão - Funcionário: {employeeId}")]
    private static partial void LogCreatingDeletionRequest(ILogger logger, Guid employeeId);

    [LoggerMessage(LogLevel.Information, "Criando categoria de dados pessoais - Nome: {categoryName}")]
    private static partial void LogCreatingDataCategory(ILogger logger, string categoryName);

    [LoggerMessage(LogLevel.Error, "Erro ao criar entidade de privacidade - Comando: {command}")]
    private static partial void LogPrivacyCreateError(ILogger logger, string command, Exception exception);

    #endregion

    public async Task<ConsentRecordDto> Handle(CreateConsentRecordCommand request, CancellationToken cancellationToken)
    {
        LogCreatingConsentRecord(_logger, request.EmployeeId, request.ConsentCategory);

        try
        {
            var entity = new ConsentRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                ConsentCategory = request.ConsentCategory,
                ProcessingPurpose = request.ProcessingPurpose,
                ConsentGranted = request.ConsentGranted,
                ConsentDate = DateTime.UtcNow,
                ExpirationDate = request.ExpirationDate,
                IsActive = true,
                TermsVersion = request.TermsVersion,
                CollectionEvidence = request.CollectionEvidence,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ConsentRecords.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToConsentRecordDto();
        }
        catch (Exception ex)
        {
            LogPrivacyCreateError(_logger, "CreateConsentRecord", ex);
            throw;
        }
    }

    public async Task<DataExportRequestDto> Handle(CreateDataExportRequestCommand request, CancellationToken cancellationToken)
    {
        LogCreatingExportRequest(_logger, request.EmployeeId);

        try
        {
            var entity = new DataExportRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                RequestDate = DateTime.UtcNow,
                Status = Pending,
                DataCategories = string.Join(",", request.DataCategories),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DataExportRequests.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDataExportRequestDto();
        }
        catch (Exception ex)
        {
            LogPrivacyCreateError(_logger, "CreateDataExportRequest", ex);
            throw;
        }
    }

    public async Task<DataDeletionRequestDto> Handle(CreateDataDeletionRequestCommand request, CancellationToken cancellationToken)
    {
        LogCreatingDeletionRequest(_logger, request.EmployeeId);

        try
        {
            var entity = new DataDeletionRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                RequestDate = DateTime.UtcNow,
                DeletionType = Enum.Parse<SynQcore.Domain.Entities.DeletionType>(request.DeletionType),
                Reason = request.Reason,
                DataCategories = string.Join(",", request.DataCategories),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DataDeletionRequests.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDataDeletionRequestDto();
        }
        catch (Exception ex)
        {
            LogPrivacyCreateError(_logger, "CreateDataDeletionRequest", ex);
            throw;
        }
    }

    public async Task<PersonalDataCategoryDto> Handle(CreatePersonalDataCategoryCommand request, CancellationToken cancellationToken)
    {
        LogCreatingDataCategory(_logger, request.CategoryName);

        try
        {
            var entity = new PersonalDataCategory
            {
                Id = Guid.NewGuid(),
                CategoryName = request.CategoryName,
                Description = request.Description,
                SensitivityLevel = Enum.Parse<SynQcore.Domain.Entities.SensitivityLevel>(request.SensitivityLevel),
                RequiresConsent = request.RequiresConsent,
                IsActive = true,
                EffectiveDate = request.EffectiveDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PersonalDataCategories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToPersonalDataCategoryDto();
        }
        catch (Exception ex)
        {
            LogPrivacyCreateError(_logger, "CreatePersonalDataCategory", ex);
            throw;
        }
    }
}
