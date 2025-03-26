using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatanaClient.API.Authentication;
using WatanaClient.API.Exceptions;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Models.Responses;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Services.Implementation;

/// <summary>
/// Implementación principal del servicio de comunicación con la API de Watana.
/// Esta clase maneja todas las comunicaciones HTTP con el servidor, incluyendo
/// serialización, deserialización y manejo de errores.
/// </summary>
public class WatanaService : IWatanaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WatanaService> _logger;
    private readonly IFileService _fileService;
    private readonly AuthenticationOptions _options;
    
    /// <summary>
    /// Opciones de serialización JSON utilizadas para todas las operaciones.
    /// Configura el nombrado en camelCase y desactiva la indentación para optimizar el tamaño.
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Inicializa una nueva instancia de WatanaService.
    /// </summary>
    /// <param name="httpClient">Cliente HTTP para realizar las solicitudes.</param>
    /// <param name="options">Opciones de autenticación para la API.</param>
    /// <param name="fileService">Servicio para manejo de archivos.</param>
    /// <param name="logger">Logger para registro de operaciones.</param>
    public WatanaService(
        HttpClient httpClient,
        IOptions<AuthenticationOptions> options,
        IFileService fileService,
        ILogger<WatanaService> logger)
    {
        _httpClient = httpClient;
        _fileService = fileService;
        _logger = logger;
        _options = options.Value;
        
        ConfigureHttpClient();
    }
    
    /// <summary>
    /// Configura el cliente HTTP con las opciones de autenticación y cabeceras necesarias.
    /// </summary>
    /// <remarks>
    /// Este método configura:
    /// - La URL base de la API
    /// - El timeout para las solicitudes
    /// - Las cabeceras de aceptación de contenido
    /// - El token de autorización
    /// </remarks>
    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_options.Url);
        _httpClient.Timeout = _options.Timeout;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", _options.Token);
    }

    /// <summary>
    /// Envía una solicitud a la API y deserializa la respuesta al tipo especificado.
    /// </summary>
    /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
    /// <param name="operacion">El tipo de operación a realizar.</param>
    /// <param name="data">Los datos de la solicitud.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La respuesta deserializada al tipo especificado.</returns>
    /// <exception cref="WatanaException">
    /// Se lanza cuando hay errores de deserialización o comunicación con el servidor.
    /// </exception>
    /// <remarks>
    /// Este método maneja automáticamente:
    /// - La serialización de la solicitud
    /// - El envío de la petición HTTP
    /// - La deserialización de la respuesta
    /// - El manejo de errores y excepciones
    /// </remarks>
    public async Task<T> EnviarSolicitudAsync<T>(string operacion, WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        var jsonResponse = await EnviarSolicitudRawAsync(operacion, data, cancellationToken);
        
        try
        {
            var result = JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions);
            return result ?? throw new WatanaException($"Error al deserializar la respuesta a {typeof(T).Name}");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar la respuesta JSON: {Message}", ex.Message);
            throw new WatanaException("Error al procesar la respuesta del servidor", ex);
        }
    }

    /// <summary>
    /// Envía una solicitud a la API y retorna la respuesta como JSON sin procesar.
    /// </summary>
    /// <param name="operacion">El tipo de operación a realizar.</param>
    /// <param name="data">Los datos de la solicitud.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La respuesta del servidor como cadena JSON.</returns>
    /// <exception cref="WatanaException">
    /// Se lanza en los siguientes casos:
    /// - Error de comunicación con el servidor
    /// - Timeout en la solicitud
    /// - Respuesta HTTP no exitosa
    /// - Tipo de contenido no soportado
    /// </exception>
    /// <remarks>
    /// Este método maneja automáticamente:
    /// - La serialización de la solicitud
    /// - El envío de la petición HTTP
    /// - El procesamiento de respuestas ZIP (archivos descargados)
    /// - La conversión de archivos binarios a Base64
    /// - El manejo de errores y excepciones
    /// </remarks>
    public async Task<string> EnviarSolicitudRawAsync(string operacion, WatanaApiObject data, CancellationToken cancellationToken = default)
    {
        if (data.Operacion == null)
        {
            data.Operacion = operacion;
        }
        
        try
        {
            var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Enviando solicitud a {Url} con operación {Operacion}", _httpClient.BaseAddress, operacion);
            
            var response = await _httpClient.PostAsync("", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Error HTTP {StatusCode}: {Content}", response.StatusCode, errorContent);
                throw new WatanaException($"Error HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
            
            var contentType = response.Content.Headers.ContentType?.MediaType;
            
            if (contentType == "application/json")
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            else if (contentType == "application/x-zip-compressed")
            {
                var zipBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                var zipBase64 = await _fileService.ConvertirABase64Async(zipBytes, cancellationToken);
                
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = (contentDisposition?.FileName ?? "archivo_sin_nombre.zip").Trim('"');
                fileName = Path.GetFileNameWithoutExtension(fileName);
                
                var responseDescarga = new DescargaResponse
                {
                    Success = true,
                    Mensaje = "Archivo descargado correctamente",
                    SolicitudNumero = "00000000000",
                    Archivos = new List<ArchivoResponse>
                    {
                        new()
                        {
                            Nombre = fileName,
                            ZipBase64 = zipBase64
                        }
                    }
                };
                
                return JsonSerializer.Serialize(responseDescarga, _jsonOptions);
            }
            else
            {
                throw new WatanaException($"Tipo de contenido no soportado: {contentType}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error en la solicitud HTTP: {Message}", ex.Message);
            throw new WatanaException("Error al comunicarse con el servidor", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout en la solicitud HTTP");
            throw new WatanaException("Tiempo de espera agotado para la solicitud", ex);
        }
        catch (Exception ex) when (ex is not WatanaException)
        {
            _logger.LogError(ex, "Error inesperado: {Message}", ex.Message);
            throw new WatanaException("Error inesperado al procesar la solicitud", ex);
        }
    }
}