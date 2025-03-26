namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la información de un archivo en las respuestas del API.
/// Este record contiene los datos del archivo comprimido y codificado.
/// </summary>
public record ArchivoResponse
{
    /// <summary>
    /// Obtiene el nombre original del archivo.
    /// Incluye la extensión del archivo.
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el contenido del archivo comprimido y codificado en Base64.
    /// Este contenido puede ser descomprimido usando el método DescomprimirDesdeBase64Async del cliente.
    /// </summary>
    public string ZipBase64 { get; init; } = string.Empty;
}