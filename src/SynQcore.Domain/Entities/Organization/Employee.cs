namespace SynQcore.Domain.Entities.Organization;

/// <summary>
/// Representa um funcionário na organização corporativa.
/// Entidade central do sistema que gerencia identificação, dados pessoais, profissionais e relacionamentos hierárquicos.
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>
    /// Identificador único do funcionário na organização (ex: matrícula, código corporativo).
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de email corporativo do funcionário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Primeiro nome do funcionário.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Sobrenome do funcionário.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone corporativo ou pessoal.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// URL da foto do perfil corporativo.
    /// </summary>
    public string? ProfilePhotoUrl { get; set; }

    /// <summary>
    /// Biografia ou descrição profissional do funcionário.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Título do cargo ou função desempenhada.
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Posição hierárquica na estrutura corporativa.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Data de contratação do funcionário.
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Indica se o funcionário está ativo na organização.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ID do gestor direto na hierarquia organizacional.
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// Gestor direto do funcionário na hierarquia.
    /// </summary>
    public Employee? Manager { get; set; }

    /// <summary>
    /// Nome completo concatenando primeiro nome e sobrenome.
    /// </summary>
    public string FullName =>
        $"{FirstName}{LastName}".Trim();

    /// <summary>
    /// Nome para exibição, usando nome completo ou email como fallback.
    /// </summary>
    public string DisplayName =>
        string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)
        ? Email
        : FullName;

    /// <summary>
    /// Número de anos de serviço na organização.
    /// </summary>
    public int YearsOfService =>
        DateTime.UtcNow.Year - HireDate.Year;

    /// <summary>
    /// Funcionários subordinados diretos na hierarquia.
    /// </summary>
    public ICollection<Employee> Subordinates { get; set; } = [];

    /// <summary>
    /// Relacionamentos com departamentos organizacionais.
    /// </summary>
    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = [];

    /// <summary>
    /// Participações em equipes corporativas.
    /// </summary>
    public ICollection<TeamMembership> TeamMemberships { get; set; } = [];

    /// <summary>
    /// Relacionamentos hierárquicos como subordinado.
    /// </summary>
    public ICollection<ReportingRelationship> DirectReports { get; set; } = [];

    /// <summary>
    /// Relacionamentos hierárquicos como gestor.
    /// </summary>
    public ICollection<ReportingRelationship> ManagerRelationships { get; set; } = [];

    /// <summary>
    /// Posts criados pelo funcionário na plataforma.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = [];

    /// <summary>
    /// Comentários feitos pelo funcionário.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// Curtidas em posts realizadas pelo funcionário.
    /// </summary>
    public ICollection<PostLike> PostLikes { get; set; } = [];

    /// <summary>
    /// Curtidas em comentários realizadas pelo funcionário.
    /// </summary>
    public ICollection<CommentLike> CommentLikes { get; set; } = [];

    /// <summary>
    /// Menções feitas pelo funcionário a outros colegas.
    /// </summary>
    public ICollection<CommentMention> MentionsMade { get; set; } = [];

    /// <summary>
    /// Notificações recebidas pelo funcionário.
    /// </summary>
    public ICollection<Notification> ReceivedNotifications { get; set; } = [];

    /// <summary>
    /// Notificações enviadas pelo funcionário.
    /// </summary>
    public ICollection<Notification> SentNotifications { get; set; } = [];
}
