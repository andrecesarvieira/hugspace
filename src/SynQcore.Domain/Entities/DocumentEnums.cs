namespace SynQcore.Domain.Entities;

/// <summary>
/// Tipos de documentos corporativos suportados pelo sistema
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// Política corporativa oficial
    /// </summary>
    Policy = 1,

    /// <summary>
    /// Procedimento operacional padrão
    /// </summary>
    Procedure = 2,

    /// <summary>
    /// Manual de funcionário
    /// </summary>
    Manual = 3,

    /// <summary>
    /// Formulário corporativo
    /// </summary>
    Form = 4,

    /// <summary>
    /// Template/modelo corporativo
    /// </summary>
    Template = 5,

    /// <summary>
    /// Apresentação corporativa
    /// </summary>
    Presentation = 6,

    /// <summary>
    /// Planilha ou dados
    /// </summary>
    Spreadsheet = 7,

    /// <summary>
    /// Contrato ou documento legal
    /// </summary>
    Contract = 8,

    /// <summary>
    /// Certificado ou licença
    /// </summary>
    Certificate = 9,

    /// <summary>
    /// Imagem ou asset visual
    /// </summary>
    Image = 10,

    /// <summary>
    /// Vídeo corporativo
    /// </summary>
    Video = 11,

    /// <summary>
    /// Arquivo de áudio
    /// </summary>
    Audio = 12,

    /// <summary>
    /// Arquivo compactado
    /// </summary>
    Archive = 13,

    /// <summary>
    /// Documento geral/outros
    /// </summary>
    General = 14
}

/// <summary>
/// Status do documento no workflow corporativo
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// Rascunho - ainda sendo editado
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Aguardando revisão/aprovação
    /// </summary>
    PendingReview = 2,

    /// <summary>
    /// Em processo de revisão
    /// </summary>
    InReview = 3,

    /// <summary>
    /// Aprovado e ativo
    /// </summary>
    Approved = 4,

    /// <summary>
    /// Rejeitado - precisa de correções
    /// </summary>
    Rejected = 5,

    /// <summary>
    /// Arquivado - não mais ativo
    /// </summary>
    Archived = 6,

    /// <summary>
    /// Expirado - passou da validade
    /// </summary>
    Expired = 7,

    /// <summary>
    /// Suspenso temporariamente
    /// </summary>
    Suspended = 8,

    /// <summary>
    /// Obsoleto - substituído por nova versão
    /// </summary>
    Obsolete = 9
}

/// <summary>
/// Níveis de acesso para documentos corporativos
/// </summary>
public enum DocumentAccessLevel
{
    /// <summary>
    /// Público - todos os funcionários podem acessar
    /// </summary>
    Public = 1,

    /// <summary>
    /// Interno - apenas funcionários da empresa
    /// </summary>
    Internal = 2,

    /// <summary>
    /// Departamental - apenas funcionários do departamento
    /// </summary>
    Departmental = 3,

    /// <summary>
    /// Restrito - apenas usuários com permissão específica
    /// </summary>
    Restricted = 4,

    /// <summary>
    /// Confidencial - apenas níveis executivos
    /// </summary>
    Confidential = 5,

    /// <summary>
    /// Secreto - acesso extremamente limitado
    /// </summary>
    Secret = 6
}

/// <summary>
/// Categorias de documentos para organização
/// </summary>
public enum DocumentCategory
{
    /// <summary>
    /// Recursos Humanos
    /// </summary>
    HumanResources = 1,

    /// <summary>
    /// Financeiro/Contábil
    /// </summary>
    Financial = 2,

    /// <summary>
    /// Legal/Jurídico
    /// </summary>
    Legal = 3,

    /// <summary>
    /// Tecnologia da Informação
    /// </summary>
    Technology = 4,

    /// <summary>
    /// Operacional
    /// </summary>
    Operations = 5,

    /// <summary>
    /// Marketing/Vendas
    /// </summary>
    Marketing = 6,

    /// <summary>
    /// Qualidade/Processos
    /// </summary>
    Quality = 7,

    /// <summary>
    /// Segurança/Compliance
    /// </summary>
    Security = 8,

    /// <summary>
    /// Treinamento/Educação
    /// </summary>
    Training = 9,

    /// <summary>
    /// Projetos
    /// </summary>
    Projects = 10,

    /// <summary>
    /// Administrativa
    /// </summary>
    Administrative = 11,

    /// <summary>
    /// Executiva/Estratégica
    /// </summary>
    Executive = 12
}

/// <summary>
/// Tipos de acesso que podem ser concedidos a documentos
/// </summary>
public enum AccessType
{
    /// <summary>
    /// Apenas visualizar o documento
    /// </summary>
    Read = 1,

    /// <summary>
    /// Visualizar e fazer download
    /// </summary>
    Download = 2,

    /// <summary>
    /// Visualizar, download e comentar
    /// </summary>
    Comment = 3,

    /// <summary>
    /// Editar o documento (nova versão)
    /// </summary>
    Edit = 4,

    /// <summary>
    /// Controle total (incluindo deletar)
    /// </summary>
    FullControl = 5
}

/// <summary>
/// Ações que podem ser realizadas em documentos (para auditoria)
/// </summary>
public enum DocumentAction
{
    /// <summary>
    /// Visualização do documento
    /// </summary>
    View = 1,

    /// <summary>
    /// Download do documento
    /// </summary>
    Download = 2,

    /// <summary>
    /// Upload de nova versão
    /// </summary>
    Upload = 3,

    /// <summary>
    /// Edição de metadados
    /// </summary>
    Edit = 4,

    /// <summary>
    /// Compartilhamento do documento
    /// </summary>
    Share = 5,

    /// <summary>
    /// Remoção/exclusão
    /// </summary>
    Delete = 6,

    /// <summary>
    /// Arquivamento
    /// </summary>
    Archive = 7,

    /// <summary>
    /// Aprovação do documento
    /// </summary>
    Approve = 8,

    /// <summary>
    /// Rejeição do documento
    /// </summary>
    Reject = 9,

    /// <summary>
    /// Comentário adicionado
    /// </summary>
    Comment = 10,

    /// <summary>
    /// Acesso negado (tentativa sem permissão)
    /// </summary>
    AccessDenied = 11
}

/// <summary>
/// Tipos de assets de mídia corporativa
/// </summary>
public enum MediaAssetType
{
    /// <summary>
    /// Imagem (JPEG, PNG, GIF, etc.)
    /// </summary>
    Image = 1,

    /// <summary>
    /// Vídeo (MP4, AVI, MOV, etc.)
    /// </summary>
    Video = 2,

    /// <summary>
    /// Áudio (MP3, WAV, AAC, etc.)
    /// </summary>
    Audio = 3,

    /// <summary>
    /// Logo da empresa
    /// </summary>
    Logo = 4,

    /// <summary>
    /// Ícone corporativo
    /// </summary>
    Icon = 5,

    /// <summary>
    /// Banner ou imagem promocional
    /// </summary>
    Banner = 6,

    /// <summary>
    /// Avatar ou foto de funcionário
    /// </summary>
    Avatar = 7,

    /// <summary>
    /// Screenshot ou captura de tela
    /// </summary>
    Screenshot = 8,

    /// <summary>
    /// Diagrama ou infográfico
    /// </summary>
    Diagram = 9,

    /// <summary>
    /// Documento digitalizado
    /// </summary>
    Scan = 10
}

/// <summary>
/// Categorias de assets de mídia para organização
/// </summary>
public enum MediaAssetCategory
{
    /// <summary>
    /// Branding corporativo (logos, cores, fontes)
    /// </summary>
    Branding = 1,

    /// <summary>
    /// Marketing e comunicação
    /// </summary>
    Marketing = 2,

    /// <summary>
    /// Recursos Humanos
    /// </summary>
    HumanResources = 3,

    /// <summary>
    /// Treinamento e educação
    /// </summary>
    Training = 4,

    /// <summary>
    /// Eventos corporativos
    /// </summary>
    Events = 5,

    /// <summary>
    /// Produtos e serviços
    /// </summary>
    Products = 6,

    /// <summary>
    /// Instalações e escritórios
    /// </summary>
    Facilities = 7,

    /// <summary>
    /// Equipe e funcionários
    /// </summary>
    Team = 8,

    /// <summary>
    /// Projetos e desenvolvimentos
    /// </summary>
    Projects = 9,

    /// <summary>
    /// Documentação técnica
    /// </summary>
    Technical = 10,

    /// <summary>
    /// Templates e modelos
    /// </summary>
    Templates = 11,

    /// <summary>
    /// Geral/outros
    /// </summary>
    General = 12
}