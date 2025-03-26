namespace WatanaClient.API.Exceptions;

/// <summary>
/// Excepción base para todos los errores relacionados con la API de Watana.
/// Proporciona diferentes constructores para manejar diversos escenarios de error.
/// </summary>
public class WatanaException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaException"/> con un mensaje predeterminado.
    /// </summary>
    public WatanaException() : base("Ha ocurrido un error en la API de Watana")
    {
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaException"/> con un mensaje específico.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    public WatanaException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaException"/> con un mensaje específico y una excepción interna.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    /// <param name="innerException">La excepción que causó este error.</param>
    public WatanaException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción que se lanza cuando ocurren errores de validación en las operaciones de la API.
/// Esta excepción se utiliza principalmente para validaciones de datos y reglas de negocio.
/// </summary>
public class WatanaValidationException : WatanaException
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaValidationException"/> con un mensaje específico.
    /// </summary>
    /// <param name="message">El mensaje que describe el error de validación.</param>
    public WatanaValidationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Excepción que se lanza cuando ocurren errores de autenticación con la API.
/// Esta excepción se utiliza cuando hay problemas con las credenciales o el token de acceso.
/// </summary>
public class WatanaAuthenticationException : WatanaException
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaAuthenticationException"/> con un mensaje predeterminado.
    /// </summary>
    public WatanaAuthenticationException()
        : base("Error de autenticación: La URL o TOKEN no son correctos, por favor verifique")
    {
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaAuthenticationException"/> con un mensaje específico.
    /// </summary>
    /// <param name="message">El mensaje que describe el error de autenticación.</param>
    public WatanaAuthenticationException(string message) : base(message)
    {
    }
}