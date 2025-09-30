/*
 * SynQcore - Corporate Social Network
 *
 * Input Sanitization Service Implementation - Advanced security protection
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Services;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de sanitização de entrada para segurança corporativa
/// </summary>
public partial class InputSanitizationService : IInputSanitizationService
{
    private readonly ILogger<InputSanitizationService> _logger;

    // JsonSerializerOptions reutilizável para evitar warning CA1869
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    // Generated Regex patterns para alta performance
    [GeneratedRegex(@"<script[^>]*>.*?</script>|javascript:|vbscript:|onload=|onerror=|onclick=|onmouseover=|<iframe|<object|<embed", RegexOptions.IgnoreCase)]
    private static partial Regex XssPattern();

    [GeneratedRegex(@"(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE)?|INSERT( +INTO)?|MERGE|SELECT|UPDATE|UNION( +ALL)?)\b)|('|('')|;)|(--)|(/\*)|(\*/)", RegexOptions.IgnoreCase)]
    private static partial Regex SqlInjectionPattern();

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailPattern();

    [GeneratedRegex(@"^[a-zA-Z0-9._-]+$")]
    private static partial Regex UsernamePattern();

    [GeneratedRegex(@"[\x00-\x1F\x7F-\x9F]")]
    private static partial Regex ControlCharacterPattern();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();

    [GeneratedRegex(@"[';]", RegexOptions.IgnoreCase)]
    private static partial Regex SqlQuotesPattern();

    [GeneratedRegex(@"--.*$", RegexOptions.Multiline)]
    private static partial Regex SqlCommentsPattern();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex SqlBlockCommentsPattern();

    [GeneratedRegex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ScriptTagPattern();

    [GeneratedRegex(@"\s*on\w+\s*=\s*[""'][^""']*[""']", RegexOptions.IgnoreCase)]
    private static partial Regex EventHandlerPattern();

    [GeneratedRegex(@"(javascript|vbscript|data):", RegexOptions.IgnoreCase)]
    private static partial Regex MaliciousUriPattern();

    [GeneratedRegex(@"password[""':\s]*[""']([^""']*)[""']", RegexOptions.IgnoreCase)]
    private static partial Regex PasswordPattern();

    [GeneratedRegex(@"token[""':\s]*[""']([^""']*)[""']", RegexOptions.IgnoreCase)]
    private static partial Regex TokenPattern();

    [GeneratedRegex(@"authorization[""':\s]*[""']([^""']*)[""']", RegexOptions.IgnoreCase)]
    private static partial Regex AuthorizationPattern();

    [GeneratedRegex(@"[<>""';\\]", RegexOptions.Compiled)]
    private static partial Regex DangerousCharactersPattern();

    [GeneratedRegex(@"[^a-zA-Z0-9._-]", RegexOptions.Compiled)]
    private static partial Regex InvalidUsernameCharactersPattern();

    // Lista de palavras-chave perigosas para SQL injection
    private static readonly string[] SqlKeywords = {
        "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "EXECUTE",
        "UNION", "DECLARE", "CAST", "CONVERT", "SUBSTRING", "ASCII", "CHAR", "NCHAR",
        "SYSOBJECTS", "SYSCOLUMNS", "INFORMATION_SCHEMA", "xp_", "sp_"
    };

    // Lista de tags HTML perigosas
    private static readonly string[] DangerousHtmlTags = {
        "script", "iframe", "object", "embed", "form", "input", "link", "meta", "style",
        "base", "applet", "body", "frame", "frameset", "head", "html", "layer", "title"
    };

    public InputSanitizationService(ILogger<InputSanitizationService> logger)
    {
        _logger = logger;
    }

    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        try
        {
            // Remove scripts e tags perigosas
            var sanitized = input;

            // Remove tags script completos
            sanitized = ScriptTagPattern().Replace(sanitized, "");

            // Remove event handlers
            sanitized = EventHandlerPattern().Replace(sanitized, "");

            // Remove javascript: e vbscript: URIs
            sanitized = MaliciousUriPattern().Replace(sanitized, "");

            // Remove tags perigosas
            foreach (var tag in DangerousHtmlTags)
            {
                sanitized = Regex.Replace(sanitized, $@"<{tag}[^>]*>.*?</{tag}>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                sanitized = Regex.Replace(sanitized, $@"<{tag}[^>]*/>", "", RegexOptions.IgnoreCase);
            }

            LogSanitizationPerformed(_logger, "HTML", input.Length, sanitized.Length, null);
            return sanitized.Trim();
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "HTML", ex.Message, ex);
            return string.Empty;
        }
    }
    public string SanitizeText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        try
        {
            var sanitized = input;

            // Remove caracteres de controle
            sanitized = RemoveControlCharacters(sanitized);

            // Remove tentativas básicas de SQL injection
            sanitized = SqlQuotesPattern().Replace(sanitized, "");

            // Remove comentários SQL
            sanitized = SqlCommentsPattern().Replace(sanitized, "");
            sanitized = SqlBlockCommentsPattern().Replace(sanitized, "");

            // Normaliza espaços
            sanitized = NormalizeWhitespace(sanitized);

            return sanitized.Trim();
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "Text", ex.Message, ex);
            return string.Empty;
        }
    }

    public string SanitizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        try
        {
            var sanitized = email.Trim().ToLowerInvariant();

            // Remove caracteres especiais perigosos
            sanitized = DangerousCharactersPattern().Replace(sanitized, "");

            // Valida formato básico
            if (!EmailPattern().IsMatch(sanitized))
            {
                LogInvalidEmailFormat(_logger, email, null);
                return string.Empty;
            }

            return sanitized;
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "Email", ex.Message, ex);
            return string.Empty;
        }
    }

    public string SanitizeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return string.Empty;

        try
        {
            var sanitized = username.Trim();

            // Remove caracteres não permitidos
            sanitized = InvalidUsernameCharactersPattern().Replace(sanitized, "");

            // Limita tamanho
            if (sanitized.Length > 50)
                sanitized = sanitized[..50];

            return sanitized;
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "Username", ex.Message, ex);
            return string.Empty;
        }
    }

    public bool ContainsXssAttempt(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var hasXss = XssPattern().IsMatch(input);

        if (hasXss)
        {
            LogSecurityThreatDetected(_logger, "XSS", input.Length, null);
        }

        return hasXss;
    }

    public bool ContainsSqlInjectionAttempt(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Verifica padrões regex
        if (SqlInjectionPattern().IsMatch(input))
        {
            LogSecurityThreatDetected(_logger, "SQL Injection", input.Length, null);
            return true;
        }        // Verifica palavras-chave perigosas
        var inputUpper = input.ToUpperInvariant();
        foreach (var keyword in SqlKeywords)
        {
            if (inputUpper.Contains(keyword))
            {
                LogSecurityThreatDetected(_logger, $"SQL Keyword: {keyword}", input.Length, null);
                return true;
            }
        }

        return false;
    }

    public string SanitizeForLogging(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "[empty]";

        try
        {
            var sanitized = input;

            // Remove informações sensíveis comuns
            sanitized = PasswordPattern().Replace(sanitized, "password=\"***\"");
            sanitized = TokenPattern().Replace(sanitized, "token=\"***\"");
            sanitized = AuthorizationPattern().Replace(sanitized, "authorization=\"***\"");

            // Limita tamanho para logs
            if (sanitized.Length > 200)
                sanitized = sanitized[..200] + "...";

            return sanitized;
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "Logging", ex.Message, ex);
            return "[error sanitizing]";
        }
    }

    public string SanitizeJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            // Tenta parsear e reformatar para validar estrutura
            var jsonDocument = JsonDocument.Parse(json);
            var sanitized = JsonSerializer.Serialize(jsonDocument, _jsonOptions);

            return sanitized;
        }
        catch (JsonException ex)
        {
            LogInvalidJsonFormat(_logger, json.Length, ex.Message, ex);
            return string.Empty;
        }
        catch (Exception ex)
        {
            LogSanitizationError(_logger, "JSON", ex.Message, ex);
            return string.Empty;
        }
    }

    public string RemoveControlCharacters(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return ControlCharacterPattern().Replace(input, "");
    }

    public string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return WhitespacePattern().Replace(input.Trim(), " ");
    }

    // LoggerMessage delegates para performance
    [LoggerMessage(EventId = 5001, Level = LogLevel.Information,
        Message = "Input sanitization performed - Type: {Type} | OriginalLength: {OriginalLength} | SanitizedLength: {SanitizedLength}")]
    private static partial void LogSanitizationPerformed(ILogger logger, string type, int originalLength, int sanitizedLength, Exception? exception);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Warning,
        Message = "Security threat detected - Type: {ThreatType} | InputLength: {InputLength}")]
    private static partial void LogSecurityThreatDetected(ILogger logger, string threatType, int inputLength, Exception? exception);

    [LoggerMessage(EventId = 5003, Level = LogLevel.Error,
        Message = "Sanitization error - Type: {Type} | Error: {ErrorMessage}")]
    private static partial void LogSanitizationError(ILogger logger, string type, string errorMessage, Exception? exception);

    [LoggerMessage(EventId = 5004, Level = LogLevel.Warning,
        Message = "Invalid email format detected - Email: {Email}")]
    private static partial void LogInvalidEmailFormat(ILogger logger, string email, Exception? exception);

    [LoggerMessage(EventId = 5005, Level = LogLevel.Warning,
        Message = "Invalid JSON format detected - Length: {Length} | Error: {Error}")]
    private static partial void LogInvalidJsonFormat(ILogger logger, int length, string error, Exception? exception);
}
