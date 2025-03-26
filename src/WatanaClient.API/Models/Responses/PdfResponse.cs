namespace WatanaClient.API.Models.Responses;

/// <summary>
/// Representa la respuesta a operaciones realizadas sobre documentos PDF.
/// Este record contiene el resultado de operaciones como firma, sellado o validación de PDFs.
/// </summary>
public record PdfResponse
{
    /// <summary>
    /// Obtiene un valor que indica si la operación sobre el PDF fue exitosa.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Obtiene el mensaje descriptivo del resultado de la operación.
    /// Puede contener detalles del éxito o error del procesamiento del PDF.
    /// </summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene el archivo PDF procesado.
    /// Este campo puede ser nulo si la operación no fue exitosa o no generó un archivo de salida.
    /// </summary>
    public ArchivoResponse? Archivo { get; init; }
}