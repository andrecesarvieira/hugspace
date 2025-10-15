using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Extensions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.DocumentTemplates.Commands;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.DocumentTemplates.Handlers;

/// <summary>
/// Handler para criar template
/// </summary>
public partial class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, DocumentTemplateDto>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateTemplateCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Criando novo template: {Name}")]
    private static partial void LogCriandoTemplate(ILogger logger, string name, Exception? exception);

    public CreateTemplateCommandHandler(ISynQcoreDbContext context, ILogger<CreateTemplateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentTemplateDto> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        LogCriandoTemplate(_logger, request.Name, null);

        var template = new DocumentTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TemplateFileName = $"{Guid.NewGuid()}.docx",
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            Version = "1.0",
            DefaultCategory = Enum.Parse<DocumentCategory>(request.Category, true),
            DefaultAccessLevel = request.DefaultAccessLevel,
            DocumentType = DocumentType.Template,
            FileSizeBytes = request.Content.Length,
            Placeholders = string.Join(",", request.Fields.Select(f => f.Name)),
            IsActive = request.IsActive,
            UsageCount = 0,
            CreatedByEmployeeId = Guid.NewGuid(), // TODO: Obter do contexto de usuário
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.DocumentTemplates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);

        return template.ToDocumentTemplateDto();
    }
}

/// <summary>
/// Handler para atualizar template
/// </summary>
public partial class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, DocumentTemplateDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UpdateTemplateCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Atualizando template: {TemplateId}")]
    private static partial void LogAtualizandoTemplate(ILogger logger, Guid templateId, Exception? exception);

    public UpdateTemplateCommandHandler(ISynQcoreDbContext context, ILogger<UpdateTemplateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentTemplateDto?> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        LogAtualizandoTemplate(_logger, request.Id, null);

        var template = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (template == null) return null;

        if (!string.IsNullOrEmpty(request.Name))
            template.Name = request.Name;

        if (request.Description != null)
            template.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            template.DefaultCategory = Enum.Parse<DocumentCategory>(request.Category, true);

        if (request.DefaultAccessLevel.HasValue)
            template.DefaultAccessLevel = request.DefaultAccessLevel.Value;

        if (request.Fields != null)
            template.Placeholders = string.Join(",", request.Fields.Select(f => f.Name));

        if (request.IsActive.HasValue)
            template.IsActive = request.IsActive.Value;

        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return template.ToDocumentTemplateDto();
    }
}

/// <summary>
/// Handler para deletar template
/// </summary>
public partial class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, bool>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DeleteTemplateCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Deletando template: {TemplateId}")]
    private static partial void LogDeletandoTemplate(ILogger logger, Guid templateId, Exception? exception);

    public DeleteTemplateCommandHandler(ISynQcoreDbContext context, ILogger<DeleteTemplateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {
        LogDeletandoTemplate(_logger, request.TemplateId, null);

        var template = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, cancellationToken);

        if (template == null) return false;

        template.IsDeleted = true;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

/// <summary>
/// Handler para criar documento a partir de template
/// </summary>
public partial class CreateDocumentFromTemplateCommandHandler : IRequestHandler<CreateDocumentFromTemplateCommand, CreateDocumentFromTemplateDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<CreateDocumentFromTemplateCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Criando documento a partir do template: {TemplateId}")]
    private static partial void LogCriandoDocumentoTemplate(ILogger logger, Guid templateId, Exception? exception);

    public CreateDocumentFromTemplateCommandHandler(ISynQcoreDbContext context, ILogger<CreateDocumentFromTemplateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateDocumentFromTemplateDto?> Handle(CreateDocumentFromTemplateCommand request, CancellationToken cancellationToken)
    {
        LogCriandoDocumentoTemplate(_logger, request.TemplateId, null);

        var template = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId && !t.IsDeleted && t.IsActive, cancellationToken);

        if (template == null) return null;

        // Simular geração de conteúdo processado
        var processedContent = $"Documento gerado a partir do template: {template.Name}";
        if (request.FieldValues.Count > 0)
        {
            processedContent += "\n\nCampos preenchidos:\n";
            foreach (var field in request.FieldValues)
            {
                processedContent += $"- {field.Key}: {field.Value}\n";
            }
        }

        // Incrementar contador de uso
        template.UsageCount++;
        template.LastUsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateDocumentFromTemplateDto
        {
            DocumentId = Guid.NewGuid(),
            DocumentTitle = request.Title,
            GeneratedContent = processedContent,
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Handler para duplicar template
/// </summary>
public partial class DuplicateTemplateCommandHandler : IRequestHandler<DuplicateTemplateCommand, DocumentTemplateDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<DuplicateTemplateCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Duplicando template: {TemplateId}")]
    private static partial void LogDuplicandoTemplate(ILogger logger, Guid templateId, Exception? exception);

    public DuplicateTemplateCommandHandler(ISynQcoreDbContext context, ILogger<DuplicateTemplateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentTemplateDto?> Handle(DuplicateTemplateCommand request, CancellationToken cancellationToken)
    {
        LogDuplicandoTemplate(_logger, request.SourceTemplateId, null);

        var originalTemplate = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.SourceTemplateId && !t.IsDeleted, cancellationToken);

        if (originalTemplate == null) return null;

        var newTemplate = new DocumentTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.NewName,
            Description = request.NewDescription ?? originalTemplate.Description,
            TemplateFileName = $"{Guid.NewGuid()}.docx",
            ContentType = originalTemplate.ContentType,
            Version = "1.0",
            DefaultCategory = originalTemplate.DefaultCategory,
            DefaultAccessLevel = originalTemplate.DefaultAccessLevel,
            DocumentType = originalTemplate.DocumentType,
            FileSizeBytes = originalTemplate.FileSizeBytes,
            Placeholders = originalTemplate.Placeholders,
            UsageInstructions = originalTemplate.UsageInstructions,
            Tags = originalTemplate.Tags,
            IsActive = true,
            UsageCount = 0,
            CreatedByEmployeeId = Guid.NewGuid(), // TODO: Obter do contexto de usuário
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.DocumentTemplates.Add(newTemplate);
        await _context.SaveChangesAsync(cancellationToken);

        return newTemplate.ToDocumentTemplateDto();
    }
}

/// <summary>
/// Handler para alterar status do template
/// </summary>
public partial class ToggleTemplateStatusCommandHandler : IRequestHandler<ToggleTemplateStatusCommand, DocumentTemplateDto?>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<ToggleTemplateStatusCommandHandler> _logger;

    [LoggerMessage(LogLevel.Information, "Alterando status do template: {TemplateId}")]
    private static partial void LogAlterandoStatusTemplate(ILogger logger, Guid templateId, Exception? exception);

    public ToggleTemplateStatusCommandHandler(ISynQcoreDbContext context, ILogger<ToggleTemplateStatusCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DocumentTemplateDto?> Handle(ToggleTemplateStatusCommand request, CancellationToken cancellationToken)
    {
        LogAlterandoStatusTemplate(_logger, request.TemplateId, null);

        var template = await _context.DocumentTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId && !t.IsDeleted, cancellationToken);

        if (template == null) return null;

        template.IsActive = !template.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return template.ToDocumentTemplateDto();
    }
}
