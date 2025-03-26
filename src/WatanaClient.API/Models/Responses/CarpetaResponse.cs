namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la respuesta a operaciones relacionadas con carpetas de documentos.
/// Este record contiene información sobre el estado y contenido de una carpeta.
/// </summary>
public record CarpetaResponse
{
    /// <summary>
    /// Obtiene un valor que indica si la operación fue exitosa.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Obtiene el mensaje descriptivo del resultado de la operación.
    /// Puede contener detalles del éxito o error de la operación.
    /// </summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el código único que identifica la carpeta.
    /// Este código se utiliza para operaciones posteriores con la carpeta.
    /// </summary>
    public string CarpetaCodigo { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el estado actual de la carpeta.
    /// Puede indicar estados como "Activa", "Eliminada", "En Proceso", etc.
    /// </summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene la fecha y hora de creación de la carpeta.
    /// </summary>
    public DateTime FechaCreacion { get; init; }

    /// <summary>
    /// Obtiene la lista de archivos contenidos en la carpeta.
    /// Este campo puede ser nulo si la carpeta está vacía o si ocurrió un error.
    /// </summary>
    public List<ArchivoResponse>? Archivos { get; init; }
}