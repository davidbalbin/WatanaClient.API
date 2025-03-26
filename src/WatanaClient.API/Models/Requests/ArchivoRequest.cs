namespace WatanaClient.API.Models.Requests;

/// <summary>
/// Representa un archivo que será procesado para firma digital.
/// Este record contiene la información del archivo comprimido y codificado.
/// </summary>
public record ArchivoRequest
{
    /// <summary>
    /// Obtiene el nombre del archivo original.
    /// Este campo es obligatorio y debe incluir la extensión del archivo.
    /// </summary>
    public required string Nombre { get; init; }

    /// <summary>
    /// Obtiene el contenido del archivo comprimido y codificado en Base64.
    /// Este campo es obligatorio y debe ser generado usando el método ComprimirYConvertirABase64Async del cliente.
    /// </summary>
    public required string ZipBase64 { get; init; }
}