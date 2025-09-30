using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Features.Moderation.Utilities;

/// <summary>
/// Utilitários para mapeamento de entidades de auditoria para DTOs
/// </summary>
public static class ModerationMappingUtilities
{
    /// <summary>
    /// Mapeia AuditLogEntity para ModerationAuditLogDto
    /// </summary>
    public static ModerationAuditLogDto MapToModerationDto(AuditLogEntity log)
    {
        return new ModerationAuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.UserName,
            UserRole = log.UserRole,
            ActionType = log.ActionType,
            ActionDescription = GetActionDescription(log.ActionType),
            ResourceType = log.ResourceType,
            ResourceId = log.ResourceId,
            Details = log.Details,
            Success = log.Success,
            ErrorMessage = log.ErrorMessage,
            ClientIpAddress = log.ClientIpAddress,
            UserAgent = log.UserAgent,
            Severity = log.Severity,
            SeverityDescription = GetSeverityDescription(log.Severity),
            Category = log.Category,
            CategoryDescription = GetCategoryDescription(log.Category),
            CreatedAt = log.CreatedAt,
            RequiresAttention = log.RequiresAttention,
            ReviewedAt = log.ReviewedAt,
            ReviewedBy = log.ReviewedBy,
            TimeAgo = GetTimeAgo(log.CreatedAt),
            SeverityCssClass = GetSeverityCssClass(log.Severity),
            ActionIcon = GetActionIcon(log.ActionType)
        };
    }

    private static string GetActionDescription(AuditActionType actionType)
    {
        return actionType switch
        {
            AuditActionType.Login => "Login de usuário",
            AuditActionType.Logout => "Logout de usuário",
            AuditActionType.LoginFailed => "Tentativa de login falhada",
            AuditActionType.Create => "Criação de registro",
            AuditActionType.Update => "Atualização de registro",
            AuditActionType.Delete => "Exclusão de registro",
            AuditActionType.Read => "Leitura de dados",
            AuditActionType.PermissionGranted => "Permissão concedida",
            AuditActionType.PermissionDenied => "Permissão negada",
            AuditActionType.SecurityViolation => "Violação de segurança",
            AuditActionType.DataExport => "Exportação de dados",
            AuditActionType.DataDeletion => "Exclusão de dados",
            AuditActionType.UnauthorizedAccess => "Acesso não autorizado",
            AuditActionType.SuspiciousActivity => "Atividade suspeita",
            _ => actionType.ToString()
        };
    }

    private static string GetSeverityDescription(AuditSeverity severity)
    {
        return severity switch
        {
            AuditSeverity.Information => "Informação",
            AuditSeverity.Warning => "Aviso",
            AuditSeverity.Error => "Erro",
            AuditSeverity.Critical => "Crítico",
            AuditSeverity.Security => "Segurança",
            _ => severity.ToString()
        };
    }

    private static string GetCategoryDescription(AuditCategory category)
    {
        return category switch
        {
            AuditCategory.Authentication => "Autenticação",
            AuditCategory.Authorization => "Autorização",
            AuditCategory.DataAccess => "Acesso a Dados",
            AuditCategory.DataModification => "Modificação de Dados",
            AuditCategory.SystemOperation => "Operação do Sistema",
            AuditCategory.Security => "Segurança",
            AuditCategory.Compliance => "Compliance",
            AuditCategory.Administration => "Administração",
            AuditCategory.FileManagement => "Gerenciamento de Arquivos",
            AuditCategory.Communication => "Comunicação",
            AuditCategory.Reporting => "Relatórios",
            AuditCategory.Integration => "Integração",
            AuditCategory.Performance => "Performance",
            AuditCategory.Error => "Erro",
            _ => category.ToString()
        };
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Agora mesmo";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minuto(s) atrás";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hora(s) atrás";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} dia(s) atrás";

        return dateTime.ToString("dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string GetSeverityCssClass(AuditSeverity severity)
    {
        return severity switch
        {
            AuditSeverity.Information => "badge bg-info",
            AuditSeverity.Warning => "badge bg-warning",
            AuditSeverity.Error => "badge bg-danger",
            AuditSeverity.Critical => "badge bg-dark",
            AuditSeverity.Security => "badge bg-primary",
            _ => "badge bg-secondary"
        };
    }

    private static string GetActionIcon(AuditActionType actionType)
    {
        return actionType switch
        {
            AuditActionType.Login => "bi-box-arrow-in-right",
            AuditActionType.Logout => "bi-box-arrow-left",
            AuditActionType.LoginFailed => "bi-shield-exclamation",
            AuditActionType.Create => "bi-plus-circle",
            AuditActionType.Update => "bi-pencil-square",
            AuditActionType.Delete => "bi-trash",
            AuditActionType.Read => "bi-eye",
            AuditActionType.PermissionGranted => "bi-check-circle",
            AuditActionType.PermissionDenied => "bi-x-circle",
            AuditActionType.SecurityViolation => "bi-shield-x",
            AuditActionType.DataExport => "bi-download",
            AuditActionType.DataDeletion => "bi-file-earmark-x",
            AuditActionType.UnauthorizedAccess => "bi-exclamation-triangle",
            AuditActionType.SuspiciousActivity => "bi-question-diamond",
            _ => "bi-activity"
        };
    }
}
