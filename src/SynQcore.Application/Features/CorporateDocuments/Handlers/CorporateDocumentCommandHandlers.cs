using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.CorporateDocuments.Commands;
using SynQcore.Application.Features.CorporateDocuments.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.CorporateDocuments.Handlers;

internal static partial class LogMessages
{
    [LoggerMessage(LogLevel.Information, "Criando novo documento corporativo: {Title}")]
    public static partial void LogCreatingDocument(this ILogger logger, string title);

    [LoggerMessage(LogLevel.Information, "Documento criado com sucesso: {DocumentId}")]
    public static partial void LogDocumentCreated(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Atualizando documento corporativo: {DocumentId}")]
    public static partial void LogUpdatingDocument(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Documento atualizado com sucesso: {DocumentId}")]
    public static partial void LogDocumentUpdated(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Excluindo documento corporativo: {DocumentId}")]
    public static partial void LogDeletingDocument(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Documento excluído com sucesso: {DocumentId}")]
    public static partial void LogDocumentDeleted(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Aprovando documento corporativo: {DocumentId}")]
    public static partial void LogApprovingDocument(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Documento aprovado com sucesso: {DocumentId}")]
    public static partial void LogDocumentApproved(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Rejeitando documento corporativo: {DocumentId}")]
    public static partial void LogRejectingDocument(this ILogger logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Documento rejeitado: {DocumentId}")]
    public static partial void LogDocumentRejected(this ILogger logger, Guid documentId);
}

public class CreateCorporateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, CorporateDocumentDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateCorporateDocumentCommandHandler> _logger;

    public CreateCorporateDocumentCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CreateCorporateDocumentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CorporateDocumentDto> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogCreatingDocument(request.Title);

        // Buscar o funcionário atual
        var currentEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == _currentUserService.UserId, cancellationToken);

        if (currentEmployee == null)
            throw new UnauthorizedAccessException("Funcionário não encontrado.");

        // Criar entidade do documento
        var document = new CorporateDocument
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Category = Enum.Parse<DocumentCategory>(request.Category),
            Status = request.RequiresApproval ? DocumentStatus.PendingReview : DocumentStatus.Approved,
            AccessLevel = request.AccessLevel,
            Type = DocumentType.General,
            Version = "1.0",
            FileSizeBytes = request.FileData?.Length ?? 0,
            OriginalFileName = request.FileName,
            StorageFileName = $"{Guid.NewGuid()}_{request.FileName}",
            ContentType = request.FileContentType,
            UploadedByEmployeeId = currentEmployee.Id,
            OwnerDepartmentId = request.DepartmentId,
            DownloadCount = 0,
            IsCurrentVersion = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Adicionar tags como string concatenada (usando TagIds do comando)
        if (request.TagIds?.Count > 0)
        {
            document.Tags = string.Join(",", request.TagIds.Select(id => id.ToString()));
        }

        _context.CorporateDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDocumentCreated(document.Id);

        // Retornar DTO usando método de extensão
        var documentDto = document.ToCorporateDocumentDto();

        return documentDto;
    }
}

public class UpdateCorporateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, CorporateDocumentDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateCorporateDocumentCommandHandler> _logger;

    public UpdateCorporateDocumentCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<UpdateCorporateDocumentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CorporateDocumentDto?> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogUpdatingDocument(request.Id);

        var document = await _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (document == null)
            throw new KeyNotFoundException("Documento não encontrado.");

        // Verificar permissões
        var currentEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == _currentUserService.UserId, cancellationToken);

        if (currentEmployee == null)
            throw new UnauthorizedAccessException("Funcionário não encontrado.");

        if (document.UploadedByEmployeeId != currentEmployee.Id && !_currentUserService.IsAdmin)
            throw new UnauthorizedAccessException("Você não tem permissão para editar este documento.");

        // Atualizar campos básicos
        if (!string.IsNullOrEmpty(request.Title))
        {
            document.Title = request.Title;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            document.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            document.Category = Enum.Parse<DocumentCategory>(request.Category);
        }

        if (request.AccessLevel.HasValue)
        {
            document.AccessLevel = request.AccessLevel.Value;
        }

        document.UpdatedAt = DateTime.UtcNow;

        // Atualizar tags se fornecidas
        if (request.TagIds != null)
        {
            document.Tags = request.TagIds.Count > 0 ? string.Join(",", request.TagIds.Select(id => id.ToString())) : null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDocumentUpdated(document.Id);

        // Mapear para DTO usando método de extensão
        var documentDto = document.ToCorporateDocumentDto();

        return documentDto;
    }
}

public class DeleteCorporateDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteCorporateDocumentCommandHandler> _logger;

    public DeleteCorporateDocumentCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<DeleteCorporateDocumentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDeletingDocument(request.DocumentId);

        var document = await _context.CorporateDocuments
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

        if (document == null)
        {
            return false;
        }

        // Verificar permissões (autor ou admin)
        var currentUserId = _currentUserService.UserId;
        var currentUser = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == currentUserId, cancellationToken);

        if (currentUser?.Id != document.UploadedByEmployeeId && !_currentUserService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Não autorizado a excluir este documento");
        }

        // Remover documento
        _context.CorporateDocuments.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDocumentDeleted(request.DocumentId);

        return true;
    }
}

public class ApproveCorporateDocumentCommandHandler : IRequestHandler<ApproveDocumentCommand, CorporateDocumentDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApproveCorporateDocumentCommandHandler> _logger;

    public ApproveCorporateDocumentCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<ApproveCorporateDocumentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CorporateDocumentDto?> Handle(ApproveDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogApprovingDocument(request.DocumentId);

        var document = await _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

        if (document == null)
        {
            throw new KeyNotFoundException($"Documento não encontrado: {request.DocumentId}");
        }

        // Verificar se é admin
        if (!_currentUserService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Não autorizado a aprovar documentos");
        }

        var currentUserId = _currentUserService.UserId;
        var currentUser = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == currentUserId, cancellationToken);

        // Atualizar status e informações de aprovação
        document.Status = DocumentStatus.Approved;
        document.ApprovedByEmployeeId = currentUser?.Id;
        document.ApprovedAt = DateTimeOffset.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDocumentApproved(request.DocumentId);

        // Mapear para DTO usando método de extensão
        var documentDto = document.ToCorporateDocumentDto();

        return documentDto;
    }
}

public class RejectCorporateDocumentCommandHandler : IRequestHandler<RejectDocumentCommand, CorporateDocumentDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RejectCorporateDocumentCommandHandler> _logger;

    public RejectCorporateDocumentCommandHandler(
        ISynQcoreDbContext context,
        ICurrentUserService currentUserService,
        ILogger<RejectCorporateDocumentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CorporateDocumentDto?> Handle(RejectDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogRejectingDocument(request.DocumentId);

        var document = await _context.CorporateDocuments
            .Include(d => d.UploadedByEmployee)
            .Include(d => d.OwnerDepartment)
            .Include(d => d.ApprovedByEmployee)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

        if (document == null)
        {
            throw new KeyNotFoundException($"Documento não encontrado: {request.DocumentId}");
        }

        // Verificar se é admin
        if (!_currentUserService.IsAdmin)
        {
            throw new UnauthorizedAccessException("Não autorizado a rejeitar documentos");
        }

        var currentUserId = _currentUserService.UserId;
        var currentUser = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == currentUserId, cancellationToken);

        // Atualizar status e informações de rejeição
        document.Status = DocumentStatus.Rejected;
        document.ApprovedByEmployeeId = currentUser?.Id;
        document.ApprovedAt = DateTimeOffset.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDocumentRejected(request.DocumentId);

        // Mapear para DTO usando método de extensão
        var documentDto = document.ToCorporateDocumentDto();

        return documentDto;
    }
}
