using Microsoft.Extensions.Logging;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API;

/// <summary>
/// Implementación principal del cliente para la API de Watana.
/// Esta clase actúa como punto de entrada centralizado para todas las operaciones
/// disponibles en la API, incluyendo manejo de carpetas, solicitudes de firma y
/// operaciones con archivos PDF.
/// </summary>
public class WatanaClient : IWatanaClient
{
    private readonly IWatanaService _watanaService;
    private readonly IFileService _fileService;
    private readonly ILogger<WatanaClient> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del cliente Watana.
    /// </summary>
    /// <param name="watanaService">Servicio para comunicación con la API.</param>
    /// <param name="fileService">Servicio para manejo de archivos.</param>
    /// <param name="logger">Logger para registro de operaciones.</param>
    public WatanaClient(
        IWatanaService watanaService,
        IFileService fileService,
        ILogger<WatanaClient> logger)
    {
        _watanaService = watanaService;
        _fileService = fileService;
        _logger = logger;
    }

    #region Métodos de Carpetas

    /// <summary>
    /// Consulta el estado y contenido de una carpeta.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="carpetaCodigo">Código único de la carpeta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Información de la carpeta y su contenido.</returns>
    /// <example>
    /// <code>
    /// var resultado = await watanaClient.ConsultarCarpetaAsync&lt;CarpetaResponse&gt;("CARP001");
    /// Console.WriteLine($"Estado: {resultado.Estado}");
    /// </code>
    /// </example>
    public Task<T> ConsultarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando carpeta con código: {CarpetaCodigo}", carpetaCodigo);

        var data = new WatanaApiObject("consultar_carpeta");
        data["carpeta_codigo"] = carpetaCodigo;

        return _watanaService.EnviarSolicitudAsync<T>("consultar_carpeta", data, cancellationToken);
    }

    /// <summary>
    /// Envía una nueva carpeta con documentos al sistema.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos de la carpeta y sus documentos.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Información de la carpeta creada.</returns>
    /// <remarks>
    /// Los campos obligatorios son:
    /// - carpeta_codigo
    /// - titulo
    /// - firmante
    /// - archivos
    /// </remarks>
    public Task<T> EnviarCarpetaAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enviando carpeta con código: {CarpetaCodigo}", data.ContainsKey("carpeta_codigo") ? data["carpeta_codigo"] : "N/A");

        data.ValidarCampoObligatorio("carpeta_codigo");
        data.ValidarCampoObligatorio("titulo");
        data.ValidarCampoObligatorio("firmante");
        data.ValidarCampoObligatorio("archivos");

        return _watanaService.EnviarSolicitudAsync<T>("enviar_carpeta", data, cancellationToken);
    }

    /// <summary>
    /// Descarga los archivos de una carpeta existente.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="carpetaCodigo">Código único de la carpeta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Contenido de los archivos en la carpeta.</returns>
    public Task<T> DescargarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Descargando carpeta con código: {CarpetaCodigo}", carpetaCodigo);

        var data = new WatanaApiObject("descargar_carpeta");
        data["carpeta_codigo"] = carpetaCodigo;

        return _watanaService.EnviarSolicitudAsync<T>("descargar_carpeta", data, cancellationToken);
    }

    /// <summary>
    /// Elimina una carpeta y todo su contenido.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="carpetaCodigo">Código único de la carpeta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado de la operación de eliminación.</returns>
    /// <remarks>
    /// Esta operación es irreversible. Asegúrese de verificar que:
    /// - No haya solicitudes de firma pendientes
    /// - Se hayan respaldado los documentos importantes
    /// </remarks>
    public Task<T> EliminarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Eliminando carpeta con código: {CarpetaCodigo}", carpetaCodigo);

        var data = new WatanaApiObject("eliminar_carpeta");
        data["carpeta_codigo"] = carpetaCodigo;

        return _watanaService.EnviarSolicitudAsync<T>("eliminar_carpeta", data, cancellationToken);
    }

    #endregion

    #region Métodos de Solicitud

    /// <summary>
    /// Prepara una solicitud de firma para una carpeta existente.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos de la solicitud a preparar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Información de la solicitud preparada.</returns>
    /// <remarks>
    /// Los campos obligatorios son:
    /// - carpeta_codigo
    /// - nombre
    /// - archivos
    /// </remarks>
    public Task<T> PrepararSolicitudAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparando solicitud para carpeta: {CarpetaCodigo}", data.ContainsKey("carpeta_codigo") ? data["carpeta_codigo"] : "N/A");

        data.ValidarCampoObligatorio("carpeta_codigo");
        data.ValidarCampoObligatorio("nombre");
        data.ValidarCampoObligatorio("archivos");

        return _watanaService.EnviarSolicitudAsync<T>("preparar_solicitud", data, cancellationToken);
    }

    /// <summary>
    /// Envía una solicitud de firma a los destinatarios especificados.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos de la solicitud a enviar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Información de la solicitud enviada.</returns>
    /// <remarks>
    /// Los campos obligatorios son:
    /// - carpeta_codigo
    /// - firma_codigo
    /// - firmantes
    /// </remarks>
    public Task<T> EnviarSolicitudAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enviando solicitud para firma: {FirmaCodigo}", data.ContainsKey("firma_codigo") ? data["firma_codigo"] : "N/A");

        data.ValidarCampoObligatorio("carpeta_codigo");
        data.ValidarCampoObligatorio("firma_codigo");
        data.ValidarCampoObligatorio("firmantes");

        return _watanaService.EnviarSolicitudAsync<T>("enviar_solicitud", data, cancellationToken);
    }

    /// <summary>
    /// Consulta el estado de una solicitud de firma.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="firmaCodigo">Código único de la solicitud.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Estado actual de la solicitud.</returns>
    public Task<T> ConsultarSolicitudAsync<T>(string firmaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando solicitud con código de firma: {FirmaCodigo}", firmaCodigo);

        var data = new WatanaApiObject("consultar_solicitud");
        data["firma_codigo"] = firmaCodigo;

        return _watanaService.EnviarSolicitudAsync<T>("consultar_solicitud", data, cancellationToken);
    }

    #endregion

    #region Métodos de PDF

    /// <summary>
    /// Firma digitalmente un documento PDF.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos del PDF a firmar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>PDF firmado digitalmente.</returns>
    /// <remarks>
    /// El campo obligatorio es:
    /// - zip_base64 (contenido del PDF comprimido y codificado)
    /// </remarks>
    public Task<T> FirmarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Firmando PDF");

        data.ValidarCampoObligatorio("zip_base64");

        return _watanaService.EnviarSolicitudAsync<T>("firmar_pdf", data, cancellationToken);
    }

    /// <summary>
    /// Aplica un sello digital a un documento PDF.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos del PDF a sellar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>PDF con sello digital aplicado.</returns>
    /// <remarks>
    /// El campo obligatorio es:
    /// - zip_base64 (contenido del PDF comprimido y codificado)
    /// </remarks>
    public Task<T> SellarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sellando PDF");

        data.ValidarCampoObligatorio("zip_base64");

        return _watanaService.EnviarSolicitudAsync<T>("sellar_pdf", data, cancellationToken);
    }

    /// <summary>
    /// Valida las firmas y sellos de un documento PDF.
    /// </summary>
    /// <typeparam name="T">Tipo de respuesta esperada.</typeparam>
    /// <param name="data">Datos del PDF a validar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado de la validación del PDF.</returns>
    /// <remarks>
    /// El campo obligatorio es:
    /// - zip_base64 (contenido del PDF comprimido y codificado)
    /// </remarks>
    public Task<T> ValidarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validando PDF");

        data.ValidarCampoObligatorio("zip_base64");

        return _watanaService.EnviarSolicitudAsync<T>("validar_pdf", data, cancellationToken);
    }

    #endregion

    #region Métodos de archivo

    /// <summary>
    /// Comprime un archivo y lo convierte a formato Base64.
    /// </summary>
    /// <param name="contenido">Contenido del archivo a procesar.</param>
    /// <param name="nombreArchivo">Nombre del archivo.</param>
    /// <param name="extension">Extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Contenido comprimido y codificado en Base64.</returns>
    public async Task<string> ComprimirYConvertirABase64Async(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Comprimiendo y convirtiendo a base64 archivo: {NombreArchivo}", nombreArchivo);

        var comprimido = await _fileService.ComprimirArchivoAsync(contenido, nombreArchivo, extension, cancellationToken);
        return await _fileService.ConvertirABase64Async(comprimido, cancellationToken);
    }

    /// <summary>
    /// Decodifica un contenido Base64 y lo descomprime.
    /// </summary>
    /// <param name="base64">Contenido en formato Base64.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Contenido original descomprimido.</returns>
    public async Task<byte[]> DescomprimirDesdeBase64Async(string base64, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Descomprimiendo archivo desde base64");

        var bytes = await _fileService.ConvertirDesdeBase64Async(base64, cancellationToken);
        return await _fileService.DescomprimirArchivoAsync(bytes, cancellationToken);
    }

    #endregion
}