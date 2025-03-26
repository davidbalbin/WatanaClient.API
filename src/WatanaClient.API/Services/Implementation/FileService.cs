using System.IO.Compression;
using Microsoft.Extensions.Logging;
using WatanaClient.API.Exceptions;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Services.Implementation;

/// <summary>
/// Implementa las operaciones de manejo de archivos incluyendo compresión, descompresión
/// y codificación Base64. Esta clase proporciona diferentes métodos para procesar archivos
/// tanto en memoria como en disco.
/// </summary>
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase FileService.
    /// </summary>
    /// <param name="logger">Logger para registrar operaciones y errores.</param>
    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Comprime un archivo desde un arreglo de bytes en memoria.
    /// </summary>
    /// <param name="contenido">El contenido del archivo como arreglo de bytes.</param>
    /// <param name="nombreArchivo">Nombre del archivo sin extensión.</param>
    /// <param name="extension">Extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    /// <exception cref="WatanaException">Se lanza cuando hay errores durante la compresión.</exception>
    /// <remarks>
    /// Si no se proporciona un nombre de archivo, se generará uno aleatorio usando GUID.
    /// El archivo se comprime usando el nivel óptimo de compresión.
    /// </remarks>
    public async Task<byte[]> ComprimirArchivoAsync(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
        {
            nombreArchivo = Guid.NewGuid().ToString();
        }

        try
        {
            using var compressedFileStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {
                var entryName = string.IsNullOrEmpty(extension)
                    ? nombreArchivo
                    : $"{nombreArchivo}.{extension.TrimStart('.')}";
                    
                var zipEntry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);
                
                using var zipEntryStream = zipEntry.Open();
                await zipEntryStream.WriteAsync(contenido, cancellationToken);
            }
            
            return compressedFileStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprimir archivo {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw new WatanaException("Error al comprimir el archivo", ex);
        }
    }
    
    /// <summary>
    /// Comprime un archivo desde un stream.
    /// </summary>
    /// <param name="contenido">Stream con el contenido del archivo.</param>
    /// <param name="nombreArchivo">Nombre del archivo sin extensión.</param>
    /// <param name="extension">Extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    /// <exception cref="WatanaException">Se lanza cuando hay errores durante la compresión.</exception>
    /// <remarks>
    /// Este método copia el contenido del stream a memoria antes de comprimirlo.
    /// Para archivos grandes, considere usar <see cref="ComprimirArchivoEnDiscoAsync"/>.
    /// </remarks>
    public async Task<byte[]> ComprimirArchivoAsync(Stream contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default)
    {
        try
        {
            using var ms = new MemoryStream();
            await contenido.CopyToAsync(ms, cancellationToken);
            return await ComprimirArchivoAsync(ms.ToArray(), nombreArchivo, extension, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprimir archivo desde stream {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw new WatanaException("Error al comprimir el archivo desde stream", ex);
        }
    }

    /// <summary>
    /// Comprime un archivo usando almacenamiento temporal en disco.
    /// Este método es útil para archivos grandes que podrían causar problemas de memoria.
    /// </summary>
    /// <param name="contenido">El contenido del archivo como arreglo de bytes.</param>
    /// <param name="nombreArchivo">Nombre del archivo sin extensión.</param>
    /// <param name="extension">Extensión del archivo (opcional).</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido comprimido como arreglo de bytes.</returns>
    /// <exception cref="WatanaException">Se lanza cuando hay errores durante la compresión.</exception>
    /// <remarks>
    /// Este método:
    /// - Crea un archivo ZIP temporal en el directorio temporal del sistema
    /// - Comprime el contenido en ese archivo
    /// - Lee el resultado
    /// - Limpia el archivo temporal automáticamente
    ///
    /// Es más eficiente en memoria pero más lento debido a las operaciones de disco.
    /// </remarks>
    public async Task<byte[]> ComprimirArchivoEnDiscoAsync(byte[] contenido, string nombreArchivo, string extension = "", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
        {
            nombreArchivo = Guid.NewGuid().ToString();
        }

        var rutaTemp = Path.GetTempPath();
        var archivoZipTemp = Path.Combine(rutaTemp, $"{nombreArchivo}.zip");
        
        try
        {
            var entryName = string.IsNullOrEmpty(extension) 
                ? nombreArchivo 
                : $"{nombreArchivo}.{extension.TrimStart('.')}";
                
            using (var fileStream = new FileStream(archivoZipTemp, FileMode.Create, FileAccess.Write))
            {
                using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, false);
                var zipEntry = zipArchive.CreateEntry(entryName);
                
                using var zipEntryStream = zipEntry.Open();
                await zipEntryStream.WriteAsync(contenido, cancellationToken);
            }
            
            var resultado = await File.ReadAllBytesAsync(archivoZipTemp, cancellationToken);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprimir archivo en disco {NombreArchivo}: {Message}", nombreArchivo, ex.Message);
            throw new WatanaException("Error al comprimir el archivo en disco", ex);
        }
        finally
        {
            if (File.Exists(archivoZipTemp))
            {
                try
                {
                    File.Delete(archivoZipTemp);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo eliminar el archivo temporal: {ArchivoTemp}", archivoZipTemp);
                }
            }
        }
    }
    
    /// <summary>
    /// Descomprime un archivo ZIP y retorna el contenido del primer archivo encontrado.
    /// </summary>
    /// <param name="contenidoZip">El contenido del archivo ZIP como arreglo de bytes.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>El contenido descomprimido del primer archivo en el ZIP.</returns>
    /// <exception cref="WatanaException">
    /// Se lanza cuando:
    /// - El ZIP está vacío o es inválido
    /// - Ocurren errores durante la descompresión
    /// </exception>
    /// <remarks>
    /// Este método:
    /// - Solo procesa el primer archivo en el ZIP
    /// - Asume que el ZIP contiene al menos un archivo
    /// - Opera completamente en memoria
    /// </remarks>
    public async Task<byte[]> DescomprimirArchivoAsync(byte[] contenidoZip, CancellationToken cancellationToken = default)
    {
        try
        {
            using var ms = new MemoryStream(contenidoZip);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
            
            if (archive.Entries.Count == 0)
            {
                throw new WatanaException("El archivo ZIP no contiene entradas");
            }
            
            var entry = archive.Entries[0];
            using var entryStream = entry.Open();
            using var resultStream = new MemoryStream();
            
            await entryStream.CopyToAsync(resultStream, cancellationToken);
            return resultStream.ToArray();
        }
        catch (InvalidDataException ex)
        {
            _logger.LogError(ex, "ZIP no válido: {Message}", ex.Message);
            throw new WatanaException("ZIP no válido", ex);
        }
        catch (Exception ex) when (ex is not WatanaException)
        {
            _logger.LogError(ex, "Error al descomprimir el archivo: {Message}", ex.Message);
            throw new WatanaException("Error al descomprimir el archivo", ex);
        }
    }
    
    public Task<string> ConvertirABase64Async(byte[] datos, CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(Convert.ToBase64String(datos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir a Base64: {Message}", ex.Message);
            throw new WatanaException("Error al convertir los datos a Base64", ex);
        }
    }
    
    public Task<byte[]> ConvertirDesdeBase64Async(string base64, CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(Convert.FromBase64String(base64));
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Formato de Base64 inválido: {Message}", ex.Message);
            throw new WatanaException("Formato de Base64 inválido", ex);
        }
        catch (Exception ex) when (ex is not WatanaException)
        {
            _logger.LogError(ex, "Error al convertir desde Base64: {Message}", ex.Message);
            throw new WatanaException("Error al convertir desde Base64", ex);
        }
    }
}