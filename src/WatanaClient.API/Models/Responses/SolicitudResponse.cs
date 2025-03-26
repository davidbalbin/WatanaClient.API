namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la respuesta a una operación de solicitud de firma.
/// Este record contiene el resultado de la operación y el estado actual del proceso de firma.
/// </summary>
public record SolicitudResponse
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
    /// Obtiene el código único que identifica el proceso de firma.
    /// Este código se utiliza para consultar el estado de la firma posteriormente.
    /// </summary>
    public string FirmaCodigo { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el estado actual del proceso de firma.
    /// Puede indicar estados como "Pendiente", "Firmado", "Rechazado", etc.
    /// </summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene la lista de firmantes y su estado actual en el proceso de firma.
    /// Este campo puede ser nulo si la operación no fue exitosa.
    /// </summary>
    public List<FirmanteResponse>? Firmantes { get; init; }
}