# WatanaClient.API

Cliente API para los servicios de Watana en .NET 8.

## Requisitos

- .NET 8 SDK o superior

## Instalación

### Paquete NuGet (cuando se publique)
```
dotnet add package WatanaClient.API
```

### Referencia de proyecto
```
dotnet add reference path/to/WatanaClient.API.csproj
```

## Configuración

### En appsettings.json
```json
{
  "WatanaApi": {
    "Url": "https://api.watana.com",
    "Token": "tu-token-aqui",
    "Timeout": "00:05:00"
  }
}
```

### En Program.cs o Startup.cs

```csharp
// Con IConfiguration
services.AddWatanaClient(configuration);

// Configuración programática
services.AddWatanaClient(options =>
{
    options.Url = "https://api.watana.com";
    options.Token = "tu-token-aqui";
    options.Timeout = TimeSpan.FromMinutes(5);
});
```

## Uso Básico

```csharp
// Resolver la fábrica de operaciones
var watanaFactory = serviceProvider.GetRequiredService<WatanaFactory>();

// Obtener operaciones específicas
var carpetaOps = watanaFactory.CrearCarpetaOperations();
var solicitudOps = watanaFactory.CrearSolicitudOperations();
var pdfOps = watanaFactory.CrearPdfOperations();

// Consultar una carpeta
var carpeta = await carpetaOps.ConsultarAsync("CODIGO_CARPETA");

// Firmar un PDF
var pdfBytes = await File.ReadAllBytesAsync("documento.pdf");
var resultadoFirma = await pdfOps.FirmarPdfAsync(pdfBytes, "documento.pdf");
```

## Ejemplos

### Cargar y firmar un PDF

```csharp
// Obtener operaciones de PDF
var pdfOps = watanaFactory.CrearPdfOperations();

// Leer archivo PDF
var pdfBytes = await File.ReadAllBytesAsync("documento.pdf");

// Firmar el PDF con opciones
var resultadoFirma = await pdfOps.FirmarPdfAsync(pdfBytes, "documento.pdf", options =>
{
    options["coordenadas"] = new[] { 10, 20, 100, 50 };
    options["pagina"] = 1;
});

// Guardar el PDF firmado
if (resultadoFirma.Success && resultadoFirma.Archivo != null)
{
    var pdfFirmadoBytes = await pdfOps.ExtraerContenidoPdfAsync(resultadoFirma.Archivo.ZipBase64);
    await File.WriteAllBytesAsync("documento_firmado.pdf", pdfFirmadoBytes);
}
```

### Crear y enviar una carpeta con archivos

```csharp
// Obtener operaciones de carpeta
var carpetaOps = watanaFactory.CrearCarpetaOperations();

// Preparar archivo
var pdfBytes = await File.ReadAllBytesAsync("documento.pdf");
var archivo = await carpetaOps.CrearArchivoAsync("CARPETA_123", "contrato.pdf", pdfBytes);

// Crear carpeta con archivo
var nuevaCarpeta = await carpetaOps.EnviarAsync(new CarpetaRequest
{
    CarpetaCodigo = "CARPETA_123",
    Titulo = "Contratos 2025",
    Firmante = new FirmanteRequest
    {
        Nombre = "Juan Pérez",
        Email = "juan.perez@ejemplo.com",
        Telefono = "+5491112345678"
    },
    Archivos = new List<ArchivoRequest> { archivo }
});
```

### Preparar y enviar una solicitud de firma

```csharp
// Obtener operaciones de solicitud
var solicitudOps = watanaFactory.CrearSolicitudOperations();

// Preparar solicitud
var solicitudPreparada = await solicitudOps.PrepararAsync(
    "CARPETA_123", 
    "Solicitud de firma de contrato",
    new[] { "contrato.pdf" });

// Extraer código de firma
string firmaCodigo = solicitudPreparada.firma_codigo;

// Enviar solicitud a firmantes
var solicitudEnviada = await solicitudOps.EnviarAsync(new SolicitudRequest
{
    CarpetaCodigo = "CARPETA_123",
    FirmaCodigo = firmaCodigo,
    Firmantes = new List<FirmanteRequest>
    {
        new FirmanteRequest
        {
            Nombre = "Juan Pérez",
            Email = "juan.perez@ejemplo.com"
        },
        new FirmanteRequest
        {
            Nombre = "María García",
            Email = "maria.garcia@ejemplo.com"
        }
    }
});
```

## Licencia

Este proyecto está licenciado bajo los términos de la licencia MIT.