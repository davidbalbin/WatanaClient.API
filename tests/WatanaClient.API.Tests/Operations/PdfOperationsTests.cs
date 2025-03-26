using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Models.Responses;
using WatanaClient.API.Operations;
using WatanaClient.API.Services.Interfaces;
using Xunit;

namespace WatanaClient.API.Tests.Operations;

public class PdfOperationsTests
{
    private readonly Mock<IWatanaClient> _mockClient;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ILogger<PdfOperations>> _mockLogger;
    private readonly PdfOperations _pdfOperations;

    public PdfOperationsTests()
    {
        _mockClient = new Mock<IWatanaClient>();
        _mockFileService = new Mock<IFileService>();
        _mockLogger = new Mock<ILogger<PdfOperations>>();

        _pdfOperations = new PdfOperations(
            _mockClient.Object,
            _mockFileService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task FirmarPdfAsync_ConParametrosValidos_RetornaRespuestaExitosa()
    {
        // Arrange
        var pdfContent = Encoding.UTF8.GetBytes("Contenido PDF de prueba");
        var nombreArchivo = "documento.pdf";
        var base64Zip = "base64zipcodificado";

        var respuestaEsperada = new PdfResponse
        {
            Success = true,
            Mensaje = "PDF firmado correctamente",
            Archivo = new ArchivoResponse
            {
                Nombre = "documento_firmado",
                ZipBase64 = "base64ziprespuesta"
            }
        };

        _mockClient
            .Setup(c => c.ComprimirYConvertirABase64Async(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(base64Zip);

        _mockClient
            .Setup(c => c.FirmarPdfAsync<PdfResponse>(
                It.IsAny<WatanaApiObject>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaEsperada);

        // Act
        var resultado = await _pdfOperations.FirmarPdfAsync(pdfContent, nombreArchivo);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Success);
        Assert.Equal("PDF firmado correctamente", resultado.Mensaje);
        Assert.NotNull(resultado.Archivo);

        _mockClient.Verify(
            c => c.ComprimirYConvertirABase64Async(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockClient.Verify(
            c => c.FirmarPdfAsync<PdfResponse>(
                It.Is<WatanaApiObject>(o =>
                    o.Operacion == "firmar_pdf" &&
                    o.ContainsKey("zip_base64") &&
                    o["zip_base64"].ToString() == base64Zip),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExtraerContenidoPdfAsync_ConBase64Valido_RetornaContenidoPdf()
    {
        // Arrange
        var zipBase64 = "base64zipcodificado";
        var contenidoEsperado = Encoding.UTF8.GetBytes("Contenido PDF de prueba");

        _mockClient
            .Setup(c => c.DescomprimirDesdeBase64Async(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(contenidoEsperado);

        // Act
        var resultado = await _pdfOperations.ExtraerContenidoPdfAsync(zipBase64);

        // Assert
        Assert.Equal(contenidoEsperado, resultado);

        _mockClient.Verify(
            c => c.DescomprimirDesdeBase64Async(
                zipBase64,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SellarPdfAsync_ConParametrosValidos_RetornaRespuestaExitosa()
    {
        // Arrange
        var pdfContent = Encoding.UTF8.GetBytes("Contenido PDF de prueba");
        var nombreArchivo = "documento.pdf";
        var base64Zip = "base64zipcodificado";

        var respuestaEsperada = new PdfResponse
        {
            Success = true,
            Mensaje = "PDF sellado correctamente",
            Archivo = new ArchivoResponse
            {
                Nombre = "documento_sellado",
                ZipBase64 = "base64ziprespuesta"
            }
        };

        _mockClient
            .Setup(c => c.ComprimirYConvertirABase64Async(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(base64Zip);

        _mockClient
            .Setup(c => c.SellarPdfAsync<PdfResponse>(
                It.IsAny<WatanaApiObject>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaEsperada);

        // Act
        var resultado = await _pdfOperations.SellarPdfAsync(pdfContent, nombreArchivo);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Success);
        Assert.Equal("PDF sellado correctamente", resultado.Mensaje);
        Assert.NotNull(resultado.Archivo);

        _mockClient.Verify(
            c => c.ComprimirYConvertirABase64Async(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mockClient.Verify(
            c => c.SellarPdfAsync<PdfResponse>(
                It.Is<WatanaApiObject>(o =>
                    o.Operacion == "sellar_pdf" &&
                    o.ContainsKey("zip_base64")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}