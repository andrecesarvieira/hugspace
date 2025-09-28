namespace SynQcore.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso solicitado não é encontrado.
/// Tipicamente mapeada para HTTP 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância com mensagem de erro.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o recurso não encontrado.</param>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Inicializa uma nova instância com mensagem e exceção interna.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o recurso não encontrado.</param>
    /// <param name="innerException">Exceção que causou esta exceção.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
