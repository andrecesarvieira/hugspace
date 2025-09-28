namespace SynQcore.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando ocorrem erros de validação de dados.
/// Tipicamente mapeada para HTTP 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância com mensagem de erro de validação.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o erro de validação.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância com mensagem e exceção interna.
    /// </summary>
    /// <param name="message">Mensagem descrevendo o erro de validação.</param>
    /// <param name="innerException">Exceção que causou esta exceção.</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
