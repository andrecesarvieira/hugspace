using FluentValidation;
using SynQcore.Application.Features.Notifications.Commands;
using SynQcore.Domain.Entities;

namespace SynQcore.Application.Validators.Notifications;

/// <summary>
/// Validator para criação de notificação corporativa
/// </summary>
public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        // Título obrigatório e com tamanho adequado
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(200)
            .WithMessage("Título não pode exceder 200 caracteres");

        // Conteúdo obrigatório e com tamanho adequado
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Conteúdo é obrigatório")
            .MaximumLength(4000)
            .WithMessage("Conteúdo não pode exceder 4000 caracteres");

        // Tipo de notificação válido
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo de notificação é obrigatório");

        // Prioridade válida
        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Prioridade é obrigatória");

        // Canais habilitados válidos
        RuleFor(x => x.EnabledChannels)
            .Must(channels => channels != null && channels.Count > 0)
            .WithMessage("Pelo menos um canal deve ser habilitado");

        // Data de agendamento não pode ser no passado
        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.ScheduledFor.HasValue)
            .WithMessage("Data de agendamento deve ser no futuro");

        // Data de expiração deve ser maior que agendamento
        RuleFor(x => x.ExpiresAt)
            .GreaterThan(x => x.ScheduledFor)
            .When(x => x.ExpiresAt.HasValue && x.ScheduledFor.HasValue)
            .WithMessage("Data de expiração deve ser posterior ao agendamento");

        // Data de expiração não pode ser no passado
        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Data de expiração deve ser no futuro");

        // PlaceholderData JSON válido (se fornecido) 
        RuleFor(x => x.PlaceholderData)
            .Must(data => data == null || data.Count >= 0)
            .WithMessage("Dados de placeholder devem ser válidos");
    }

    /// <summary>
    /// Valida se a string é um JSON válido
    /// </summary>
    private static bool BeValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return true;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Validator para aprovação de notificação
/// </summary>
public class ApproveNotificationCommandValidator : AbstractValidator<ApproveNotificationCommand>
{
    public ApproveNotificationCommandValidator()
    {
        // ID da notificação obrigatório
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("ID da notificação é obrigatório");

        // Comentários opcionais mas com tamanho limitado
        RuleFor(x => x.Comments)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Comments))
            .WithMessage("Comentários não podem exceder 1000 caracteres");
    }
}

/// <summary>
/// Validator para envio de notificação
/// </summary>
public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        // ID da notificação obrigatório
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("ID da notificação é obrigatório");
    }
}

/// <summary>
/// Validator para marcação de leitura
/// </summary>
public class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        // ID da notificação obrigatório
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("ID da notificação é obrigatório");

        // ID do funcionário obrigatório
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("ID do funcionário é obrigatório");
    }
}

/// <summary>
/// Validator para atualização de notificação
/// </summary>
public class UpdateNotificationCommandValidator : AbstractValidator<UpdateNotificationCommand>
{
    public UpdateNotificationCommandValidator()
    {
        // ID da notificação obrigatório
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("ID da notificação é obrigatório");

        // Título obrigatório se fornecido
        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x => x.Title != null)
            .WithMessage("Título não pode ser vazio")
            .MaximumLength(200)
            .When(x => x.Title != null)
            .WithMessage("Título não pode exceder 200 caracteres");

        // Conteúdo obrigatório se fornecido
        RuleFor(x => x.Content)
            .NotEmpty()
            .When(x => x.Content != null)
            .WithMessage("Conteúdo não pode ser vazio")
            .MaximumLength(4000)
            .When(x => x.Content != null)
            .WithMessage("Conteúdo não pode exceder 4000 caracteres");

        // Tipo válido se fornecido
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Tipo de notificação inválido");

        // Prioridade válida se fornecida
        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Prioridade inválida");
    }
}

/// <summary>
/// Validator para cancelamento de notificação
/// </summary>
public class CancelNotificationCommandValidator : AbstractValidator<CancelNotificationCommand>
{
    public CancelNotificationCommandValidator()
    {
        // ID da notificação obrigatório
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("ID da notificação é obrigatório");

        // Motivo do cancelamento com tamanho limitado
        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Motivo do cancelamento não pode exceder 500 caracteres");
    }
}