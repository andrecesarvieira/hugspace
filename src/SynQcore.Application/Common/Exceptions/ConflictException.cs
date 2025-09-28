namespace SynQcore.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre um conflito de estado ou recurso.
/// Tipicamente mapeada para HTTP 409 Conflict.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância com mensagem de erro.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o conflito encontrado.</param>
    public ConflictException(string message) : base(message) { }

    /// <summary>
    /// Inicializa uma nova instância com mensagem e exceção interna.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o conflito encontrado.</param>
    /// <param name="innerException">Exceção que causou esta exceção.</param>
    public ConflictException(string message, Exception innerException) : base(message, innerException) { }
}
