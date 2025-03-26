using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using WatanaClient.API.Exceptions;
using WatanaClient.API.Services.Implementation;
using Xunit;

namespace WatanaClient.API.Tests.Services;

public class FileServiceTests
{
    private readonly Mock<ILogger<FileService>> _mockLogger;
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        _mockLogger = new Mock<ILogger<FileService>>();
        _fileService = new FileService(_mockLogger.Object);
    }

    [Fact]
    public async Task ComprimirArchivoAsync_ConContenidoYNombre_RetornaArchivoComprimido()
    {
        // Arrange
        var contenido = Encoding.UTF8.GetBytes("Contenido de prueba");
        var nombreArchivo = "archivo_prueba";
        var extension = "txt";

        // Act
        var resultado = await _fileService.ComprimirArchivoAsync(contenido, nombreArchivo, extension);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Length > 0);
    }

    [Fact]
    public async Task ConvertirABase64Async_ConDatosValidos_RetornaBase64Correcto()
    {
        // Arrange
        var datos = Encoding.UTF8.GetBytes("Contenido de prueba");
        var base64Esperado = Convert.ToBase64String(datos);

        // Act
        var resultado = await _fileService.ConvertirABase64Async(datos);

        // Assert
        Assert.Equal(base64Esperado, resultado);
    }

    [Fact]
    public async Task ConvertirDesdeBase64Async_ConBase64Valido_RetornaDatosCorrectos()
    {
        // Arrange
        var datos = Encoding.UTF8.GetBytes("Contenido de prueba");
        var base64 = Convert.ToBase64String(datos);

        // Act
        var resultado = await _fileService.ConvertirDesdeBase64Async(base64);

        // Assert
        Assert.Equal(datos, resultado);
    }

    [Fact]
    public async Task ConvertirDesdeBase64Async_ConBase64Invalido_LanzaExcepcion()
    {
        // Arrange
        var base64Invalido = "Este no es base64 válido!";

        // Act & Assert
        await Assert.ThrowsAsync<WatanaException>(() => 
            _fileService.ConvertirDesdeBase64Async(base64Invalido));
    }

    [Fact]
    public async Task ComprimirYDescomprimirArchivoAsync_DebeRecuperarContenidoOriginal()
    {
        // Arrange
        var contenidoOriginal = Encoding.UTF8.GetBytes("Contenido de prueba");
        var nombreArchivo = "archivo_prueba";
        var extension = "txt";

        // Act - Comprimir
        var comprimido = await _fileService.ComprimirArchivoAsync(contenidoOriginal, nombreArchivo, extension);
        
        // Act - Descomprimir
        var descomprimido = await _fileService.DescomprimirArchivoAsync(comprimido);

        // Assert
        Assert.Equal(contenidoOriginal, descomprimido);
    }
}