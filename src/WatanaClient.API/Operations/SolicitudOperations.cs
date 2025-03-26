using Microsoft.Extensions.Logging;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Models.Requests;
using WatanaClient.API.Models.Responses;

namespace WatanaClient.API.Operations;

/// <summary>
/// Implementa las operaciones relacionadas con solicitudes de firma digital.
/// Esta clase proporciona métodos para crear, enviar y consultar solicitudes
/// de firma para documentos almacenados en carpetas.
/// </summary>
public class SolicitudOperations
{
private readonly IWatanaClient _client;
private readonly ILogger _logger;

/// <summary>
/// Inicializa una nueva instancia de la clase SolicitudOperations.
/// </summary>
/// <param name="client">Cliente para comunicación con la API de Watana.</param>
/// <param name="logger">Logger para registrar operaciones y errores.</param>
public SolicitudOperations(
    IWatanaClient client,
    ILogger<SolicitudOperations> logger)
{
    _client = client;
    _logger = logger;
}

/// <summary>
/// Consulta el estado actual de una solicitud de firma.
/// </summary>
/// <param name="firmaCodigo">Código único que identifica la solicitud de firma.</param>
/// <param name="cancellationToken">Token para cancelar la operación.</param>
/// <returns>Un objeto <see cref="SolicitudResponse"/> que contiene el estado actual de la solicitud.</returns>
/// <remarks>
/// Este método permite verificar:
/// - El estado general de la solicitud
/// - La lista de firmantes y sus estados individuales
/// - Los documentos incluidos en la solicitud
/// </remarks>
/// <example>
/// Ejemplo de uso:
/// <code>
/// var solicitudOperations = serviceProvider.GetRequiredService&lt;SolicitudOperations&gt;();
/// var resultado = await solicitudOperations.ConsultarAsync("FIRM001");
/// if (resultado.Success)
/// {
///     Console.WriteLine($"Estado: {resultado.Estado}");
///     foreach (var firmante in resultado.Firmantes)
///     {
///         Console.WriteLine($"{firmante.Nombre}: {firmante.Estado}");
///     }
/// }
/// </code>
/// </example>
public Task<SolicitudResponse> ConsultarAsync(string firmaCodigo, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Consultando solicitud: {FirmaCodigo}", firmaCodigo);
    return _client.ConsultarSolicitudAsync<SolicitudResponse>(firmaCodigo, cancellationToken);
}

/// <summary>
/// Prepara una solicitud de firma para documentos contenidos en una carpeta.
/// </summary>
/// <param name="carpetaCodigo">Código de la carpeta que contiene los documentos.</param>
/// <param name="nombre">Nombre descriptivo para la solicitud.</param>
/// <param name="archivos">Lista de nombres de archivos a incluir en la solicitud.</param>
/// <param name="configurationAction">Acción opcional para configurar parámetros adicionales.</param>
/// <param name="cancellationToken">Token para cancelar la operación.</param>
/// <returns>Objeto dinámico con la información de la solicitud preparada.</returns>
/// <remarks>
/// Este método:
/// - Verifica que los archivos existan en la carpeta
/// - Prepara los documentos para el proceso de firma
/// - Genera un código único para la solicitud
/// 
/// La configuración adicional puede incluir:
/// - Orden de firma requerido
/// - Fecha límite para firmar
/// - Mensaje personalizado para los firmantes
/// </remarks>
/// <example>
/// Ejemplo de uso:
/// <code>
/// var archivos = new[] { "documento1.pdf", "documento2.pdf" };
/// var resultado = await solicitudOperations.PrepararAsync(
///     "CARP001",
///     "Contratos para firma",
///     archivos,
///     config =>
///     {
///         config["orden_requerido"] = true;
///         config["dias_expiracion"] = 5;
///     });
/// </code>
/// </example>
public async Task<dynamic> PrepararAsync(
    string carpetaCodigo,
    string nombre,
    IEnumerable<string> archivos,
    Action<WatanaApiObject>? configurationAction = null,
    CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Preparando solicitud '{Nombre}' para carpeta {CarpetaCodigo}", nombre, carpetaCodigo);

    var data = new WatanaApiObject("preparar_solicitud");
    data["carpeta_codigo"] = carpetaCodigo;
    data["nombre"] = nombre;
    data["archivos"] = archivos.ToArray();

    // Agregar configuración adicional si se proporciona
    configurationAction?.Invoke(data);

    return await _client.PrepararSolicitudAsync<dynamic>(data, cancellationToken);
}

/// <summary>
/// Envía una solicitud de firma a los destinatarios especificados.
/// </summary>
/// <param name="request">Objeto <see cref="SolicitudRequest"/> con los datos de la solicitud.</param>
/// <param name="configurationAction">Acción opcional para configurar parámetros adicionales.</param>
/// <param name="cancellationToken">Token para cancelar la operación.</param>
/// <returns>Un objeto <see cref="SolicitudResponse"/> con el resultado del envío.</returns>
/// <remarks>
/// Este método:
/// - Notifica a todos los firmantes por correo electrónico
/// - Configura los permisos de acceso a los documentos
/// - Activa el proceso de firma
/// 
/// La configuración adicional puede incluir:
/// - Plantillas de correo personalizadas
/// - Recordatorios automáticos
/// - Opciones de firma específicas por firmante
/// </remarks>
/// <example>
/// Ejemplo de uso:
/// <code>
/// var solicitud = new SolicitudRequest
/// {
///     CarpetaCodigo = "CARP001",
///     FirmaCodigo = "FIRM001",
///     Firmantes = new[]
///     {
///         new FirmanteRequest 
///         { 
///             Nombre = "Juan Pérez",
///             Email = "juan@ejemplo.com",
///             Telefono = "999888777"
///         },
///         new FirmanteRequest 
///         { 
///             Nombre = "María García",
///             Email = "maria@ejemplo.com"
///         }
///     }
/// };
/// 
/// var resultado = await solicitudOperations.EnviarAsync(solicitud);
/// if (resultado.Success)
/// {
///     Console.WriteLine("Solicitud enviada correctamente");
/// }
/// </code>
/// </example>
public async Task<SolicitudResponse> EnviarAsync(
    SolicitudRequest request,
    Action<WatanaApiObject>? configurationAction = null,
    CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Enviando solicitud {FirmaCodigo} para carpeta {CarpetaCodigo}", request.FirmaCodigo, request.CarpetaCodigo);

    var data = new WatanaApiObject("enviar_solicitud");
    data["carpeta_codigo"] = request.CarpetaCodigo;
    data["firma_codigo"] = request.FirmaCodigo;

    // Agregar firmantes
    var firmantes = new List<Dictionary<string, object>>();

    foreach (var firmante in request.Firmantes)
    {
        var firmanteObj = new Dictionary<string, object>
        {
            ["nombre"] = firmante.Nombre,
            ["email"] = firmante.Email
        };

        if (!string.IsNullOrEmpty(firmante.Telefono))
            firmanteObj["telefono"] = firmante.Telefono;

        if (!string.IsNullOrEmpty(firmante.Documento))
            firmanteObj["documento"] = firmante.Documento;

        firmantes.Add(firmanteObj);
    }

    data["firmantes"] = firmantes;

    // Agregar configuración adicional si se proporciona
    configurationAction?.Invoke(data);

    return await _client.EnviarSolicitudAsync<SolicitudResponse>(data, cancellationToken);
}
}