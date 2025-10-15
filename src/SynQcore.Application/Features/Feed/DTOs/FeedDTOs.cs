using System.ComponentModel.DataAnnotations;

namespace SynQcore.Application.Features.Feed.DTOs;

/// <summary>
/// DTO para resposta de criação de post no feed
/// </summary>
public record FeedPostDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public List<string> Tags { get; init; } = [];
    public bool IsPublic { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    // Informações do autor
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public string? AuthorDepartment { get; init; }

    // Métricas de engajamento
    public int LikeCount { get; init; }
    public int CommentCount { get; init; }
    public int ViewCount { get; init; }

    // Status do post
    public string Status { get; init; } = "Published";
    public string Type { get; init; } = "FeedPost";
    public string Visibility { get; init; } = "Company";
}

/// <summary>
/// DTO para resposta de operação de like/unlike
/// </summary>
public record PostLikeResponseDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsLiked { get; init; }
    public int TotalLikes { get; init; }
    public string ReactionType { get; init; } = "Like";
    public DateTime? LikedAt { get; init; }
}

/// <summary>
/// DTO para status de curtida de um post
/// </summary>
public record PostLikeStatusDto
{
    public Guid PostId { get; init; }
    public bool IsLiked { get; init; }
    public string? ReactionType { get; init; }
    public DateTime? LikedAt { get; init; }
    public int TotalLikes { get; init; }
}

/// <summary>
/// DTO para informações de uma curtida
/// </summary>
public record PostLikeDto
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string? EmployeeAvatar { get; init; }
    public string? EmployeeJobTitle { get; init; }
    public string ReactionType { get; init; } = "Like";
    public DateTime LikedAt { get; init; }
}

/// <summary>
/// Request para criação de post no feed
/// </summary>
public record CreateFeedPostRequest
{
    [Required(ErrorMessage = "O conteúdo é obrigatório")]
    [MinLength(1, ErrorMessage = "O conteúdo deve ter pelo menos 1 caractere")]
    [MaxLength(5000, ErrorMessage = "O conteúdo deve ter no máximo 5000 caracteres")]
    public string Content { get; set; } = string.Empty;

    public string[]? Tags { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsPublic { get; set; } = true;
}

/// <summary>
/// Request para atualização de post no feed
/// </summary>
public record UpdateFeedPostRequest
{
    [MinLength(1, ErrorMessage = "O conteúdo deve ter pelo menos 1 caractere")]
    [MaxLength(5000, ErrorMessage = "O conteúdo deve ter no máximo 5000 caracteres")]
    public string? Content { get; set; }

    public string[]? Tags { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsPublic { get; set; }
}
