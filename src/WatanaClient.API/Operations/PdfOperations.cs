using Microsoft.Extensions.Logging;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Models.Responses;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Operations;

/// <summary>
/// Implementa las operaciones relacionadas con documentos PDF como firma digital,
/// sellado y validación. Esta clase proporciona métodos para procesar documentos PDF
/// de forma segura y conforme a los estándares de firma digital.
/// </summary>
public class PdfOperations
{
    private readonly IWatanaClient _client;
    private readonly IFileService _fileService;
    private readonly ILogger<PdfOperations> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase PdfOperations.
    /// </summary>
    /// <param name="client">Cliente para comunicación con la API de Watana.</param>
    /// <param name="fileService">Servicio para manejo de archivos.</param>
    /// <param name="logger">Logger para registrar operaciones y errores.</param>
    public PdfOperations(
        IWatanaClient client,
        IFileService fileService,
        ILogger<PdfOperations> logger)
    {
        _client = client;
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Firma digitalmente un documento PDF utilizando los certificados configurados en el servidor.
    /// </summary>
    /// <param name="pdfContent">Contenido del archivo PDF a firmar.</param>
    /// <param name="nombreArchivo">Nombre del archivo sin extensión.</param>
    /// <param name="configurationAction">Acción opcional para configurar parámetros adicionales de firma.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Un objeto <see cref="PdfResponse"/> que contiene el PDF firmado y el resultado de la operación.</returns>
    /// <remarks>
    /// Este método:
    /// - Comprime el PDF y lo codifica en Base64
    /// - Envía el archivo al servidor para su firma
    /// - Maneja errores y registra el progreso
    ///
    /// La configuración adicional puede incluir:
    /// - Posición de la firma visual
    /// - Información del firmante
    /// - Tipo de certificado a utilizar
    /// </remarks>
    /// <example>
    /// Ejemplo de uso básico:
    /// <code>
    /// var pdfBytes = File.ReadAllBytes("documento.pdf");
    /// var resultado = await pdfOperations.FirmarPdfAsync(pdfBytes, "documento");
    /// if (resultado.Success)
    /// {
    ///     var pdfFirmado = await pdfOperations.ExtraerContenidoPdfAsync(resultado.Archivo.ZipBase64);
    ///     File.WriteAllBytes("documento_firmado.pdf", pdfFirmado);
    /// }
    /// </code>
    ///
    /// Ejemplo con configuración adicional:
    /// <code>
    /// var resultado = await pdfOperations.FirmarPdfAsync(pdfBytes, "documento", config =>
    /// {
    ///     config["pagina"] = 1;
    ///     config["posicion_x"] = 100;
    ///     config["posicion_y"] = 100;
    /// });
    /// </code>
    /// </example>
    public async Task<PdfResponse> FirmarPdfAsync(
        byte[] pdfContent,
        string nombreArchivo,
        Action<WatanaApiObject>? configurationAction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Comenzando firma de PDF: {NombreArchivo}", nombreArchivo);

            var zipBase64 = await _client.ComprimirYConvertirABase64Async(
                pdfContent,
                nombreArchivo,
                "pdf",
                cancellationToken);

            var data = new WatanaApiObject("firmar_pdf");
            data["zip_base64"] = zipBase64;

            // Agregar configuración adicional si se proporciona
            configurationAction?.Invoke(data);

            var resultado = await _client.FirmarPdfAsync<PdfResponse>(data, cancellationToken);

            _logger.LogInformation("PDF firmado correctamente: {NombreArchivo}", nombreArchivo);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al firmar PDF {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Aplica un sello digital a un documento PDF utilizando los certificados del servidor.
    /// </summary>
    /// <param name="pdfContent">Contenido del archivo PDF a sellar.</param>
    /// <param name="nombreArchivo">Nombre del archivo sin extensión.</param>
    /// <param name="configurationAction">Acción opcional para configurar parámetros adicionales del sellado.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Un objeto <see cref="PdfResponse"/> que contiene el PDF sellado y el resultado de la operación.</returns>
    /// <remarks>
    /// Este método:
    /// - Comprime el PDF y lo codifica en Base64
    /// - Envía el archivo al servidor para aplicar el sello
    /// - Maneja errores y registra el progreso
    ///
    /// La configuración adicional puede incluir:
    /// - Tipo de sello a aplicar
    /// - Posición del sello en el documento
    /// - Información adicional para incluir en el sello
    /// </remarks>
    /// <example>
    /// Ejemplo de uso básico:
    /// <code>
    /// var pdfBytes = File.ReadAllBytes("documento.pdf");
    /// var resultado = await pdfOperations.SellarPdfAsync(pdfBytes, "documento");
    /// if (resultado.Success)
    /// {
    ///     var pdfSellado = await pdfOperations.ExtraerContenidoPdfAsync(resultado.Archivo.ZipBase64);
    ///     File.WriteAllBytes("documento_sellado.pdf", pdfSellado);
    /// }
    /// </code>
    /// </example>
    public async Task<PdfResponse> SellarPdfAsync(
        byte[] pdfContent,
        string nombreArchivo,
        Action<WatanaApiObject>? configurationAction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Comenzando sellado de PDF: {NombreArchivo}", nombreArchivo);

            var zipBase64 = await _client.ComprimirYConvertirABase64Async(
                pdfContent,
                nombreArchivo,
                "pdf",
                cancellationToken);

            var data = new WatanaApiObject("sellar_pdf");
            data["zip_base64"] = zipBase64;

            // Agregar configuración adicional si se proporciona
            configurationAction?.Invoke(data);

            var resultado = await _client.SellarPdfAsync<PdfResponse>(data, cancellationToken);

            _logger.LogInformation("PDF sellado correctamente: {NombreArchivo}", nombreArchivo);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sellar PDF {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Valida un documento PDF
    /// </summary>
    /// <param name="pdfContent">Bytes del contenido del PDF</param>
    /// <param name="nombreArchivo">Nombre del archivo</param>
    /// <param name="configurationAction">Acción para configurar opciones adicionales</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta del servidor con la validación del PDF</returns>
    public async Task<PdfResponse> ValidarPdfAsync(
        byte[] pdfContent,
        string nombreArchivo,
        Action<WatanaApiObject>? configurationAction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validando PDF: {NombreArchivo}", nombreArchivo);

            var zipBase64 = await _client.ComprimirYConvertirABase64Async(
                pdfContent,
                nombreArchivo,
                "pdf",
                cancellationToken);

            var data = new WatanaApiObject("validar_pdf");
            data["zip_base64"] = zipBase64;

            // Agregar configuración adicional si se proporciona
            configurationAction?.Invoke(data);

            var resultado = await _client.ValidarPdfAsync<PdfResponse>(data, cancellationToken);

            _logger.LogInformation("PDF validado correctamente: {NombreArchivo}", nombreArchivo);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar PDF {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Extrae el contenido de un PDF recibido en formato Base64+ZIP
    /// </summary>
    /// <param name="zipBase64">Contenido del PDF en formato Base64+ZIP</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Contenido del PDF extraído</returns>
    public async Task<byte[]> ExtraerContenidoPdfAsync(string zipBase64, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Extrayendo contenido de PDF desde Base64+ZIP");
            return await _client.DescomprimirDesdeBase64Async(zipBase64, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer contenido de PDF: {Message}", ex.Message);
            throw;
        }
    }
}