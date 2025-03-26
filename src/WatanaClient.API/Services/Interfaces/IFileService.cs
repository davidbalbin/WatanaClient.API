namespace WatanaClient.API.Services.Interfaces;

/// <summary>
/// Define las operaciones para el manejo de archivos, incluyendo compresión y codificación.
/// Esta interfaz proporciona métodos para preparar archivos antes de enviarlos a la API
/// y procesar archivos recibidos de la API.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Comprime un archivo desde un arreglo de bytes.
    /// </summary>
    /// <param name="contenido">El contenido del archivo como arreglo de bytes.</param>
    /// <param name="nombreArchivo">El nombre del archivo sin extensión.</param>
    /// <param name="extension">La extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    Task<byte[]> ComprimirArchivoAsync(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Comprime un archivo desde un stream.
    /// </summary>
    /// <param name="contenido">El stream con el contenido del archivo.</param>
    /// <param name="nombreArchivo">El nombre del archivo sin extensión.</param>
    /// <param name="extension">La extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    Task<byte[]> ComprimirArchivoAsync(Stream contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Comprime un archivo usando almacenamiento temporal en disco.
    /// Útil para archivos grandes que no caben en memoria.
    /// </summary>
    /// <param name="contenido">El contenido del archivo como arreglo de bytes.</param>
    /// <param name="nombreArchivo">El nombre del archivo sin extensión.</param>
    /// <param name="extension">La extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    Task<byte[]> ComprimirArchivoEnDiscoAsync(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Descomprime un archivo ZIP.
    /// </summary>
    /// <param name="contenidoZip">El contenido ZIP como arreglo de bytes.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido descomprimido como arreglo de bytes.</returns>
    Task<byte[]> DescomprimirArchivoAsync(byte[] contenidoZip, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convierte datos binarios a una cadena Base64.
    /// </summary>
    /// <param name="datos">Los datos a codificar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>La cadena codificada en Base64.</returns>
    Task<string> ConvertirABase64Async(byte[] datos, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convierte una cadena Base64 a datos binarios.
    /// </summary>
    /// <param name="base64">La cadena Base64 a decodificar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Los datos decodificados como arreglo de bytes.</returns>
    Task<byte[]> ConvertirDesdeBase64Async(string base64, CancellationToken cancellationToken = default);
}