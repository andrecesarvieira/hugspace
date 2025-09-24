/*
 * SynQcore - Corporate Social Network API
 * 
 * Copyright (c) 2025 André César Vieira
 * Licensed under the MIT License
 * 
 * Project Information and Branding Constants
 */

namespace SynQcore.Shared;

/// <summary>
/// SynQcore project information and author details
/// </summary>
public static class SynQcoreInfo
{
    /// <summary>
    /// Project name
    /// </summary>
    public const string ProjectName = "SynQcore";
    
    /// <summary>
    /// Full project title
    /// </summary>
    public const string FullTitle = "SynQcore - Corporate Social Network API";
    
    /// <summary>
    /// Project description
    /// </summary>
    public const string Description = "Open Source Corporate Social Network API with Clean Architecture";
    
    /// <summary>
    /// Current version
    /// </summary>
    public const string Version = "2.1.0";
    
    /// <summary>
    /// Project author
    /// </summary>
    public const string Author = "André César Vieira";
    
    /// <summary>
    /// Author email
    /// </summary>
    public const string AuthorEmail = "andrecesarvieira@hotmail.com";
    
    /// <summary>
    /// Author GitHub profile
    /// </summary>
    public const string AuthorGitHub = "https://github.com/andrecesarvieira";
    
    /// <summary>
    /// Copyright notice
    /// </summary>
    public const string Copyright = "Copyright (c) 2025 André César Vieira";
    
    /// <summary>
    /// License type
    /// </summary>
    public const string License = "MIT License";
    
    /// <summary>
    /// Project repository URL
    /// </summary>
    public const string RepositoryUrl = "https://github.com/andrecesarvieira/synqcore";
    
    /// <summary>
    /// Project website/documentation URL
    /// </summary>
    public const string ProjectUrl = "https://github.com/andrecesarvieira/synqcore";
    
    /// <summary>
    /// Build date (updated on compilation)
    /// </summary>
    public static readonly DateTime BuildDate = DateTime.UtcNow;
    
    /// <summary>
    /// Technology stack information
    /// </summary>
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
    
    /// <summary>
    /// Project statistics and metrics
    /// </summary>
    public static class Metrics
    {
        public const int EntitiesCount = 12;
        public const int DatabaseTables = 13;
        public const int ProjectsCount = 9;
        public const string CurrentPhase = "Phase 2.1 - Corporate API Foundation";
        public const string NextPhase = "Phase 2.2 - Corporate Authentication";
    }
    
    /// <summary>
    /// Gets formatted project information
    /// </summary>
    public static string GetProjectInfo()
    {
        return $"{FullTitle} v{Version}\n" +
               $"Created by {Author} ({AuthorEmail})\n" +
               $"{Copyright}\n" +
               $"Licensed under {License}\n" +
               $"Repository: {RepositoryUrl}\n" +
               $"Built on: {BuildDate:yyyy-MM-dd HH:mm:ss} UTC";
    }
    
    /// <summary>
    /// Gets formatted technology stack information
    /// </summary>
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