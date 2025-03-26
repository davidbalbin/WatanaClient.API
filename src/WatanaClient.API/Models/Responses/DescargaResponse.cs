namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la respuesta a una solicitud de descarga de documentos.
/// Este record contiene la información de los archivos descargados y el estado de la operación.
/// </summary>
public record DescargaResponse
{
    /// <summary>
    /// Obtiene un valor que indica si la operación de descarga fue exitosa.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Obtiene el mensaje descriptivo del resultado de la operación.
    /// Puede contener detalles del éxito o error de la descarga.
    /// </summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el número de solicitud asociado a la descarga.
    /// Este número identifica únicamente la operación de descarga.
    /// </summary>
    public string SolicitudNumero { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene la lista de archivos descargados.
    /// Por defecto se inicializa como una lista vacía.
    /// </summary>
    public List<ArchivoResponse> Archivos { get; init; } = new();
}