namespace WatanaClient.API.Authentication;

/// <summary>
/// Define las opciones de configuración para la autenticación con la API de Watana.
/// Esta clase se utiliza para configurar los parámetros de conexión necesarios.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Nombre de la sección en el archivo de configuración donde se encuentran las opciones de la API.
    /// Por defecto es "WatanaApi".
    /// </summary>
    public const string ConfigurationSection = "WatanaApi";
    
    /// <summary>
    /// Obtiene o establece la URL base de la API de Watana.
    /// Esta URL debe incluir el protocolo (http/https) y el dominio completo.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el token de autenticación para la API.
    /// Este token se utiliza en el encabezado de autorización de todas las solicitudes.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el tiempo máximo de espera para las solicitudes HTTP.
    /// Por defecto es de 5 minutos.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
}