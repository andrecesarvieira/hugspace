/*
 * SynQcore - Corporate Social Network API
 * 
 * Copyright (c) 2025 André César Vieira
 * Licensed under the MIT License
 * 
 * Project Information and Branding Constants
 */

namespace SynQcore.Common;

// Informações e constantes do projeto SynQcore
public static class SynQcoreInfo
{
    // Nome do projeto
    public const string ProjectName = "SynQcore";
    
    // Título completo do projeto
    public const string FullTitle = "SynQcore - Corporate Social Network API";
    
    // Descrição do projeto
    public const string Description = "Open Source Corporate Social Network API with Clean Architecture";
    
    // Versão atual
    public const string Version = "2.1.0";
    
    // Autor do projeto
    public const string Author = "André César Vieira";
    
    // Email do autor
    public const string AuthorEmail = "andrecesarvieira@hotmail.com";
    
    // Perfil GitHub do autor
    public const string AuthorGitHub = "https://github.com/andrecesarvieira";
    
    // Aviso de direitos autorais
    public const string Copyright = "Copyright (c) 2025 André César Vieira";
    
    // Tipo de licença
    public const string License = "MIT License";
    
    // URL do repositório do projeto
    public const string RepositoryUrl = "https://github.com/andrecesarvieira/synqcore";
    
    // URL do website/documentação do projeto
    public const string ProjectUrl = "https://github.com/andrecesarvieira/synqcore";
    
    // Data de build (atualizada na compilação)
    public static readonly DateTime BuildDate = DateTime.UtcNow;
    
    // Informações da stack tecnológica
    public static class Technologies
    {
        public const string Framework = ".NET 9";
        public const string Database = "PostgreSQL 16";
        public const string Cache = "Redis 7";
        public const string Architecture = "Clean Architecture";
        public const string Patterns = "CQRS, Repository, Unit of Work";
        public const string Frontend = "Blazor Hybrid";
        public const string Containerization = "Docker & Docker Compose";
    }
    
    // Estatísticas e métricas do projeto
    public static class Metrics
    {
        public const int EntitiesCount = 12;
        public const int DatabaseTables = 13;
        public const int ProjectsCount = 9;
        public const string CurrentPhase = "Phase 2.1 - Corporate API Foundation";
        public const string NextPhase = "Phase 2.2 - Corporate Authentication";
    }
    
    // Obter informações formatadas do projeto
    public static string GetProjectInfo()
    {
        return $"{FullTitle} v{Version}\n" +
               $"Created by {Author} ({AuthorEmail})\n" +
               $"{Copyright}\n" +
               $"Licensed under {License}\n" +
               $"Repository: {RepositoryUrl}\n" +
               $"Built on: {BuildDate:yyyy-MM-dd HH:mm:ss} UTC";
    }
    
    // Obter informações formatadas da stack tecnológica
    public static string GetTechStackInfo()
    {
        return $"Technology Stack:\n" +
               $"- Framework: {Technologies.Framework}\n" +
               $"- Database: {Technologies.Database}\n" +
               $"- Cache: {Technologies.Cache}\n" +
               $"- Architecture: {Technologies.Architecture}\n" +
               $"- Patterns: {Technologies.Patterns}\n" +
               $"- Frontend: {Technologies.Frontend}\n" +
               $"- Containerization: {Technologies.Containerization}";
    }
}