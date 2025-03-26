using Microsoft.Extensions.Logging;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Models.Requests;
using WatanaClient.API.Models.Responses;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Operations;

/// <summary>
/// Implementa las operaciones relacionadas con la gestión de carpetas en la API de Watana.
/// Esta clase proporciona métodos para crear, consultar, descargar y eliminar carpetas,
/// así como para gestionar los archivos contenidos en ellas.
/// </summary>
public class CarpetaOperations
{
    private readonly IWatanaClient _client;
    private readonly IFileService _fileService;
    private readonly ILogger<CarpetaOperations> _logger;

    public CarpetaOperations(
        IWatanaClient client,
        IFileService fileService,
        ILogger<CarpetaOperations> logger)
    {
        _client = client;
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta el estado y contenido de una carpeta existente.
    /// </summary>
    /// <param name="carpetaCodigo">Código único que identifica la carpeta en el sistema.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>Un objeto <see cref="CarpetaResponse"/> que contiene la información de la carpeta,
    /// incluyendo su estado actual y lista de archivos si los tiene.</returns>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var carpetaOperations = serviceProvider.GetRequiredService&lt;CarpetaOperations&gt;();
    /// var resultado = await carpetaOperations.ConsultarAsync("CARP001");
    /// if (resultado.Success)
    /// {
    ///     Console.WriteLine($"Estado de la carpeta: {resultado.Estado}");
    /// }
    /// </code>
    /// </example>
    public Task<CarpetaResponse> ConsultarAsync(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando carpeta: {CarpetaCodigo}", carpetaCodigo);
        return _client.ConsultarCarpetaAsync<CarpetaResponse>(carpetaCodigo, cancellationToken);
    }

    /// <summary>
    /// Envía una nueva carpeta al sistema con sus archivos y configuración.
    /// </summary>
    /// <param name="request">Objeto <see cref="CarpetaRequest"/> que contiene los datos de la carpeta,
    /// incluyendo código, título opcional, información del firmante y lista de archivos.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>Un objeto <see cref="CarpetaResponse"/> que contiene la información de la carpeta creada
    /// y su estado inicial.</returns>
    /// <remarks>
    /// Este método permite crear una nueva carpeta con los siguientes elementos:
    /// - Código único de carpeta
    /// - Título descriptivo (opcional)
    /// - Información del firmante (opcional)
    /// - Lista de archivos a incluir (opcional)
    ///
    /// Los archivos deben estar previamente comprimidos y codificados en Base64 usando el método
    /// <see cref="CrearArchivoAsync"/>.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var carpetaOperations = serviceProvider.GetRequiredService&lt;CarpetaOperations&gt;();
    /// var request = new CarpetaRequest
    /// {
    ///     CarpetaCodigo = "CARP001",
    ///     Titulo = "Documentos para firma",
    ///     Firmante = new FirmanteRequest
    ///     {
    ///         Nombre = "Juan Pérez",
    ///         Email = "juan@ejemplo.com"
    ///     }
    /// };
    /// var resultado = await carpetaOperations.EnviarAsync(request);
    /// </code>
    /// </example>
    public async Task<CarpetaResponse> EnviarAsync(CarpetaRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enviando carpeta: {CarpetaCodigo}", request.CarpetaCodigo);

        var data = new WatanaApiObject("enviar_carpeta");
        data["carpeta_codigo"] = request.CarpetaCodigo;
        data["titulo"] = request.Titulo ?? "Sin título";

        // Agregar firmante
        if (request.Firmante != null)
        {
            data["firmante"] = new Dictionary<string, object>
            {
                ["nombre"] = request.Firmante.Nombre,
                ["email"] = request.Firmante.Email
            };

            if (!string.IsNullOrEmpty(request.Firmante.Telefono))
                ((Dictionary<string, object>)data["firmante"])["telefono"] = request.Firmante.Telefono;

            if (!string.IsNullOrEmpty(request.Firmante.Documento))
                ((Dictionary<string, object>)data["firmante"])["documento"] = request.Firmante.Documento;
        }

        // Agregar archivos
        if (request.Archivos != null && request.Archivos.Count > 0)
        {
            var archivos = new List<Dictionary<string, string>>();

            foreach (var archivo in request.Archivos)
            {
                archivos.Add(new Dictionary<string, string>
                {
                    ["nombre"] = archivo.Nombre,
                    ["zip_base64"] = archivo.ZipBase64
                });
            }

            data["archivos"] = archivos;
        }

        return await _client.EnviarCarpetaAsync<CarpetaResponse>(data, cancellationToken);
    }

    /// <summary>
    /// Descarga una carpeta existente con todos sus archivos.
    /// </summary>
    /// <param name="carpetaCodigo">Código único que identifica la carpeta a descargar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>Un objeto <see cref="DescargaResponse"/> que contiene la información de la carpeta
    /// y sus archivos comprimidos en formato Base64.</returns>
    /// <remarks>
    /// Los archivos descargados vienen comprimidos y codificados en Base64.
    /// Para obtener el contenido original, use el método <see cref="IWatanaClient.DescomprimirDesdeBase64Async"/>.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var carpetaOperations = serviceProvider.GetRequiredService&lt;CarpetaOperations&gt;();
    /// var descarga = await carpetaOperations.DescargarAsync("CARP001");
    /// foreach (var archivo in descarga.Archivos)
    /// {
    ///     var contenido = await watanaClient.DescomprimirDesdeBase64Async(archivo.ZipBase64);
    ///     await File.WriteAllBytesAsync(archivo.Nombre, contenido);
    /// }
    /// </code>
    /// </example>
    public Task<DescargaResponse> DescargarAsync(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Descargando carpeta: {CarpetaCodigo}", carpetaCodigo);
        return _client.DescargarCarpetaAsync<DescargaResponse>(carpetaCodigo, cancellationToken);
    }

    /// <summary>
    /// Elimina una carpeta existente del sistema.
    /// </summary>
    /// <param name="carpetaCodigo">Código único que identifica la carpeta a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>Un objeto dinámico con el resultado de la operación.</returns>
    /// <remarks>
    /// Esta operación es irreversible. Una vez eliminada la carpeta, no se podrá recuperar.
    /// Se recomienda verificar que la carpeta no tenga procesos de firma pendientes antes de eliminarla.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var carpetaOperations = serviceProvider.GetRequiredService&lt;CarpetaOperations&gt;();
    /// var resultado = await carpetaOperations.EliminarAsync("CARP001");
    /// Console.WriteLine($"Carpeta eliminada: {resultado.Success}");
    /// </code>
    /// </example>
    public Task<dynamic> EliminarAsync(string carpetaCodigo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Eliminando carpeta: {CarpetaCodigo}", carpetaCodigo);
        return _client.EliminarCarpetaAsync<dynamic>(carpetaCodigo, cancellationToken);
    }

    /// <summary>
    /// Crea una solicitud de archivo comprimido y codificado para agregar a una carpeta.
    /// </summary>
    /// <param name="carpetaCodigo">Código único que identifica la carpeta destino.</param>
    /// <param name="nombreArchivo">Nombre completo del archivo incluyendo su extensión.</param>
    /// <param name="contenido">Arreglo de bytes con el contenido del archivo.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>Un objeto <see cref="ArchivoRequest"/> que contiene el nombre del archivo y su contenido
    /// comprimido y codificado en Base64, listo para ser enviado al servidor.</returns>
    /// <remarks>
    /// Este método realiza automáticamente:
    /// - Extracción de la extensión del archivo
    /// - Compresión del contenido
    /// - Codificación en Base64
    ///
    /// El objeto retornado puede ser usado directamente en el método <see cref="EnviarAsync"/>
    /// como parte de la lista de archivos.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var carpetaOperations = serviceProvider.GetRequiredService&lt;CarpetaOperations&gt;();
    /// var contenido = await File.ReadAllBytesAsync("documento.pdf");
    /// var archivo = await carpetaOperations.CrearArchivoAsync("CARP001", "documento.pdf", contenido);
    ///
    /// var request = new CarpetaRequest
    /// {
    ///     CarpetaCodigo = "CARP001",
    ///     Archivos = new List&lt;ArchivoRequest&gt; { archivo }
    /// };
    /// await carpetaOperations.EnviarAsync(request);
    /// </code>
    /// </example>
    public async Task<ArchivoRequest> CrearArchivoAsync(
        string carpetaCodigo,
        string nombreArchivo,
        byte[] contenido,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creando archivo {NombreArchivo} para carpeta {CarpetaCodigo}", nombreArchivo, carpetaCodigo);

        var extension = Path.GetExtension(nombreArchivo);
        var nombre = Path.GetFileNameWithoutExtension(nombreArchivo);

        var zipBase64 = await _client.ComprimirYConvertirABase64Async(
            contenido,
            nombre,
            extension,
            cancellationToken);

        return new ArchivoRequest
        {
            Nombre = nombreArchivo,
            ZipBase64 = zipBase64
        };
    }
}