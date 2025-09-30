/*
 * SynQcore - Corporate Social Network
 *
 * Input Sanitization Service Interface - Security protection against XSS and injection attacks
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

namespace SynQcore.Application.Services;

/// <summary>
/// Interface para serviço de sanitização de entrada para segurança corporativa
/// </summary>
public interface IInputSanitizationService
{
    /// <summary>
    /// Sanitiza HTML removendo scripts maliciosos e mantendo formatação segura
    /// </summary>
    string SanitizeHtml(string input);

    /// <summary>
    /// Sanitiza entrada de texto removendo caracteres perigosos para SQL injection
    /// </summary>
    string SanitizeText(string input);

    /// <summary>
    /// Sanitiza email removendo caracteres inválidos
    /// </summary>
    string SanitizeEmail(string email);

    /// <summary>
    /// Sanitiza nome de usuário removendo caracteres especiais perigosos
    /// </summary>
    string SanitizeUsername(string username);

    /// <summary>
    /// Valida se o input contém tentativas de XSS
    /// </summary>
    bool ContainsXssAttempt(string input);

    /// <summary>
    /// Valida se o input contém tentativas de SQL injection
    /// </summary>
    bool ContainsSqlInjectionAttempt(string input);

    /// <summary>
    /// Sanitiza input para uso em logs (remove informações sensíveis)
    /// </summary>
    string SanitizeForLogging(string input);

    /// <summary>
    /// Valida e sanitiza JSON input
    /// </summary>
    string SanitizeJson(string json);

    /// <summary>
    /// Remove caracteres de controle e não-printáveis
    /// </summary>
    string RemoveControlCharacters(string input);

    /// <summary>
    /// Normaliza espaços em branco e quebras de linha
    /// </summary>
    string NormalizeWhitespace(string input);
}
