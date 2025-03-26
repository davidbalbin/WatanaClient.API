namespace WatanaClient.API.Models.Requests;

/// <summary>
/// Representa una solicitud de firma digital.
/// Este record contiene la información necesaria para iniciar un proceso de firma.
/// </summary>
public record SolicitudRequest
{
    /// <summary>
    /// Obtiene el código único de la carpeta que contiene los documentos a firmar.
    /// </summary>
    public required string CarpetaCodigo { get; init; }

    /// <summary>
    /// Obtiene el código único que identifica el proceso de firma.
    /// </summary>
    public required string FirmaCodigo { get; init; }

    /// <summary>
    /// Obtiene la lista de firmantes que participarán en el proceso de firma.
    /// </summary>
    public required List<FirmanteRequest> Firmantes { get; init; }
}