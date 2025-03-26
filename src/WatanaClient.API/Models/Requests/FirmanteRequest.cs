namespace WatanaClient.API.Models.Requests;

/// <summary>
/// Representa la información de un firmante en el proceso de firma digital.
/// Este record contiene los datos personales necesarios para identificar y contactar al firmante.
/// </summary>
public record FirmanteRequest
{
    /// <summary>
    /// Obtiene el nombre completo del firmante.
    /// Este campo es obligatorio.
    /// </summary>
    public required string Nombre { get; init; }

    /// <summary>
    /// Obtiene el correo electrónico del firmante.
    /// Este campo es obligatorio y se utiliza para enviar notificaciones.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Obtiene el número de teléfono del firmante.
    /// Este campo es opcional y puede utilizarse para notificaciones SMS.
    /// </summary>
    public string? Telefono { get; init; }

    /// <summary>
    /// Obtiene el número de documento de identidad del firmante.
    /// Este campo es opcional y puede utilizarse para validación adicional.
    /// </summary>
    public string? Documento { get; init; }
}