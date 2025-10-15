using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Exceptions;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Features.Employees.Commands;

namespace SynQcore.Application.Features.Employees.Handlers;

public class UploadEmployeeAvatarHandler : IRequestHandler<UploadEmployeeAvatarCommand, string>
{
    private readonly ISynQcoreDbContext _context;
    private readonly ILogger<UploadEmployeeAvatarHandler> _logger;

    private static readonly Action<ILogger, Guid, string, Exception?> LogAvatarUploaded =
        LoggerMessage.Define<Guid, string>(LogLevel.Information, new EventId(1, "AvatarUploaded"),
            "Avatar uploaded successfully for employee: {EmployeeId} - {FileName}");

    public UploadEmployeeAvatarHandler(ISynQcoreDbContext context, ILogger<UploadEmployeeAvatarHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> Handle(UploadEmployeeAvatarCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted, cancellationToken);

        if (employee == null)
            throw new NotFoundException($"Employee with ID {request.EmployeeId} not found");

        // Validar arquivo
        if (request.Avatar == null || request.Avatar.Length == 0)
            throw new ArgumentException("Avatar file is required");

        // Validar tamanho (máx 5MB)
        if (request.Avatar.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Avatar file size cannot exceed 5MB");

        // Validar tipo de arquivo
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(request.Avatar.ContentType))
            throw new ArgumentException("Only JPEG, PNG and GIF files are allowed");

        // Gerar nome único para o arquivo
        var fileName = $"{request.EmployeeId}_{Guid.NewGuid()}{Path.GetExtension(request.Avatar.FileName)}";
        var avatarUrl = $"/avatars/{fileName}";

        // TODO: Implementar upload para storage (local, S3, Azure, etc.)
        // Por agora, simular o upload
        LogAvatarUploaded(_logger, request.EmployeeId, fileName, null);

        // Atualizar URL do avatar no employee
        employee.ProfilePhotoUrl = avatarUrl;
        employee.UpdateTimestamp();

        await _context.SaveChangesAsync(cancellationToken);

        return avatarUrl;
    }
}
