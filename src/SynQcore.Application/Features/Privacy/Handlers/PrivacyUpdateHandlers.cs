using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Privacy.Commands;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Application.Features.Privacy.Utilities;

namespace SynQcore.Application.Features.Privacy.Handlers;

/// <summary>
/// Handler para comandos Update de Privacy
/// </summary>
public partial class PrivacyUpdateHandlers :
    IRequestHandler<UpdateConsentRecordCommand, ConsentRecordDto?>,
    IRequestHandler<UpdatePersonalDataCategoryCommand, PersonalDataCategoryDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<PrivacyUpdateHandlers> _logger;

    public PrivacyUpdateHandlers(
        ISynQcoreDbContext context,
        ILogger<PrivacyUpdateHandlers> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region LoggerMessage Delegates

    [LoggerMessage(LogLevel.Information, "Atualizando registro de consentimento - Id: {id}")]
    private static partial void LogUpdatingConsentRecord(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Atualizando categoria de dados pessoais - Id: {id}")]
    private static partial void LogUpdatingDataCategory(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Warning, "Entidade não encontrada para atualização - Tipo: {entityType}, Id: {id}")]
    private static partial void LogEntityNotFoundForUpdate(ILogger logger, string entityType, Guid id);

    [LoggerMessage(LogLevel.Error, "Erro ao atualizar entidade de privacidade - Comando: {command}")]
    private static partial void LogPrivacyUpdateError(ILogger logger, string command, Exception exception);

    #endregion

    public async Task<ConsentRecordDto?> Handle(UpdateConsentRecordCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingConsentRecord(_logger, request.Id);

        try
        {
            var entity = await _context.ConsentRecords
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                LogEntityNotFoundForUpdate(_logger, "ConsentRecord", request.Id);
                return null;
            }

            entity.ConsentGranted = request.ConsentGranted;
            entity.ExpirationDate = request.ExpirationDate;
            entity.Notes = request.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToConsentRecordDto();
        }
        catch (Exception ex)
        {
            LogPrivacyUpdateError(_logger, "UpdateConsentRecord", ex);
            throw;
        }
    }

    public async Task<PersonalDataCategoryDto?> Handle(UpdatePersonalDataCategoryCommand request, CancellationToken cancellationToken)
    {
        LogUpdatingDataCategory(_logger, request.Id);

        try
        {
            var entity = await _context.PersonalDataCategories
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                LogEntityNotFoundForUpdate(_logger, "PersonalDataCategory", request.Id);
                return null;
            }

            entity.CategoryName = request.CategoryName;
            entity.Description = request.Description;
            entity.SensitivityLevel = Enum.Parse<SynQcore.Domain.Entities.SensitivityLevel>(request.SensitivityLevel);
            entity.RequiresConsent = request.RequiresConsent;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToPersonalDataCategoryDto();
        }
        catch (Exception ex)
        {
            LogPrivacyUpdateError(_logger, "UpdatePersonalDataCategory", ex);
            throw;
        }
    }
}
