using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WatanaClient.API.Extensions;

/// <summary>
/// Proporciona métodos de extensión para HttpClient específicos para la API de Watana.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Opciones predeterminadas para la serialización JSON.
    /// Configura el nombrado en camelCase y desactiva la indentación para optimizar el tamaño.
    /// </summary>
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Realiza una solicitud POST con contenido JSON y deserializa la respuesta.
    /// </summary>
    /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
    /// <param name="client">El cliente HTTP.</param>
    /// <param name="requestUri">La URI del endpoint.</param>
    /// <param name="content">El objeto a serializar como contenido JSON.</param>
    /// <param name="options">Opciones de serialización JSON personalizadas (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación (opcional).</param>
    /// <returns>El objeto deserializado de tipo T o null si la respuesta no es JSON.</returns>
    /// <exception cref="HttpRequestException">Se lanza cuando la solicitud HTTP no es exitosa.</exception>
    public static async Task<T?> PostJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object content,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var jsonOptions = options ?? DefaultOptions;
        var jsonContent = JsonSerializer.Serialize(content, jsonOptions);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync(requestUri, stringContent, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        if (response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions, cancellationToken);
        }
        
        return default;
    }
    
    /// <summary>
    /// Configura las opciones predeterminadas del cliente HTTP para la API de Watana.
    /// </summary>
    /// <param name="client">El cliente HTTP a configurar.</param>
    /// <param name="baseUrl">La URL base de la API de Watana.</param>
    /// <param name="token">El token de autorización.</param>
    /// <param name="timeout">El tiempo máximo de espera para las solicitudes.</param>
    public static void ConfigureWatanaDefaults(this HttpClient client, string baseUrl, string token, TimeSpan timeout)
    {
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = timeout;
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", token);
    }
}