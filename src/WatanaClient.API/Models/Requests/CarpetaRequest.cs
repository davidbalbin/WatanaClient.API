namespace WatanaClient.API.Models.Requests;

/// <summary>
/// Representa una carpeta que contiene documentos para el proceso de firma digital.
/// Este record permite organizar y gestionar los documentos a firmar.
/// </summary>
public record CarpetaRequest
{
    /// <summary>
    /// Obtiene el código único que identifica la carpeta.
    /// Este campo es obligatorio.
    /// </summary>
    public required string CarpetaCodigo { get; init; }

    /// <summary>
    /// Obtiene el título descriptivo de la carpeta.
    /// Este campo es opcional y ayuda a identificar el propósito de la carpeta.
    /// </summary>
    public string? Titulo { get; init; }

    /// <summary>
    /// Obtiene la información del firmante asociado a la carpeta.
    /// Este campo es opcional y se utiliza cuando la carpeta está asociada a un único firmante.
    /// </summary>
    public FirmanteRequest? Firmante { get; init; }

    /// <summary>
    /// Obtiene la lista de archivos contenidos en la carpeta.
    /// Este campo es opcional al crear la carpeta, pero necesario para el proceso de firma.
    /// </summary>
    public List<ArchivoRequest>? Archivos { get; init; }
}