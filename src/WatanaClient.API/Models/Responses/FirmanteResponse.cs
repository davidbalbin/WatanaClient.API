namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la información y estado de un firmante en el proceso de firma digital.
/// Este record contiene los datos del firmante y su progreso en el proceso de firma.
/// </summary>
public record FirmanteResponse
{
    /// <summary>
    /// Obtiene el nombre completo del firmante.
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el correo electrónico del firmante.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el estado actual del firmante en el proceso.
    /// Puede indicar estados como "Pendiente", "Firmado", "Rechazado", etc.
    /// </summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene la fecha y hora en que se realizó la firma.
    /// Este valor es nulo si el documento aún no ha sido firmado por este firmante.
    /// </summary>
    public DateTime? FechaFirma { get; init; }
}