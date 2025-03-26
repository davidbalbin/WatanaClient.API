using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WatanaClient.API;
using WatanaClient.API.Extensions;
using WatanaClient.API.Models.Requests;
using WatanaClient.API.Operations;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

// Configuración desde appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Configurar servicios
var services = new ServiceCollection();

// Agregar logging
services.AddLogging(builder => 
{
    builder.AddConsole();
    builder.AddDebug();
});

// Registrar WatanaClient
services.AddWatanaClient(configuration);

// Alternativamente, configurar opciones programáticamente
/*
services.AddWatanaClient(options =>
{
    options.Url = "https://api.watana.com";
    options.Token = "tu-token-aqui";
    options.Timeout = TimeSpan.FromMinutes(5);
});
*/

// Construir proveedor de servicios
var serviceProvider = services.BuildServiceProvider();

// Resolver la fábrica de operaciones
var watanaFactory = serviceProvider.GetRequiredService<WatanaFactory>();

// Ejemplos de uso
async Task EjemplosDeUsoAsync()
{
    try
    {
        // Obtener operaciones específicas
        var carpetaOps = watanaFactory.CrearCarpetaOperations();
        var solicitudOps = watanaFactory.CrearSolicitudOperations();
        var pdfOps = watanaFactory.CrearPdfOperations();
        
        // Ejemplo 1: Consultar una carpeta
        var codigoCarpeta = "CARPETA_001";
        var carpeta = await carpetaOps.ConsultarAsync(codigoCarpeta);
        Console.WriteLine($"Carpeta consultada: {carpeta.CarpetaCodigo}, Estado: {carpeta.Estado}");
        
        // Ejemplo 2: Firmar un PDF
        var rutaPdf = "documento.pdf";
        if (File.Exists(rutaPdf))
        {
            var pdfBytes = await File.ReadAllBytesAsync(rutaPdf);
            var resultadoFirma = await pdfOps.FirmarPdfAsync(pdfBytes, "documento.pdf", options =>
            {
                options["coordenadas"] = new[] { 10, 20, 100, 50 };
                options["pagina"] = 1;
            });
            
            Console.WriteLine($"PDF firmado: {resultadoFirma.Success}, Mensaje: {resultadoFirma.Mensaje}");
            
            // Guardar el PDF firmado si existe
            if (resultadoFirma.Success && resultadoFirma.Archivo != null)
            {
                var pdfFirmadoBytes = await pdfOps.ExtraerContenidoPdfAsync(resultadoFirma.Archivo.ZipBase64);
                await File.WriteAllBytesAsync("documento_firmado.pdf", pdfFirmadoBytes);
                Console.WriteLine("PDF firmado guardado como: documento_firmado.pdf");
            }
        }
        
        // Ejemplo 3: Crear una nueva carpeta con un archivo
        if (File.Exists(rutaPdf))
        {
            var pdfBytes = await File.ReadAllBytesAsync(rutaPdf);
            var archivo = await carpetaOps.CrearArchivoAsync("CARPETA_002", "contrato.pdf", pdfBytes);
            
            var nuevaCarpeta = await carpetaOps.EnviarAsync(new CarpetaRequest
            {
                CarpetaCodigo = "CARPETA_002",
                Titulo = "Nueva carpeta de contratos",
                Firmante = new FirmanteRequest
                {
                    Nombre = "Juan Pérez",
                    Email = "juan.perez@ejemplo.com",
                    Telefono = "+5491112345678"
                },
                Archivos = new List<ArchivoRequest> { archivo }
            });
            
            Console.WriteLine($"Carpeta creada: {nuevaCarpeta.CarpetaCodigo}, Mensaje: {nuevaCarpeta.Mensaje}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Error interno: {ex.InnerException.Message}");
        }
    }
}

await EjemplosDeUsoAsync();