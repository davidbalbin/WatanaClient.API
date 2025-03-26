using WatanaClient.API.Models.Common;

namespace WatanaClient.API.Interfaces;

/// <summary>
/// Interfaz para el cliente principal de Watana API
/// </summary>
public interface IWatanaClient
{
    /// <summary>
    /// Consulta una carpeta existente
    /// </summary>
    Task<T> ConsultarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una carpeta con información y archivos
    /// </summary>
    Task<T> EnviarCarpetaAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Descarga una carpeta existente
    /// </summary>
    Task<T> DescargarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una carpeta existente
    /// </summary>
    Task<T> EliminarCarpetaAsync<T>(string carpetaCodigo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prepara una solicitud de firma
    /// </summary>
    Task<T> PrepararSolicitudAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una solicitud de firma
    /// </summary>
    Task<T> EnviarSolicitudAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta una solicitud de firma existente
    /// </summary>
    Task<T> ConsultarSolicitudAsync<T>(string firmaCodigo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Firma un documento PDF
    /// </summary>
    Task<T> FirmarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sella un documento PDF
    /// </summary>
    Task<T> SellarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida un documento PDF
    /// </summary>
    Task<T> ValidarPdfAsync<T>(WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Comprime contenido y lo convierte a Base64
    /// </summary>
    Task<string> ComprimirYConvertirABase64Async(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Descomprime contenido desde Base64
    /// </summary>
    Task<byte[]> DescomprimirDesdeBase64Async(string base64, CancellationToken cancellationToken = default);
}