using Microsoft.Extensions.DependencyInjection;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Operations;

namespace WatanaClient.API;

/// <summary>
/// Fábrica para crear operaciones específicas de Watana
/// </summary>
public class WatanaFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WatanaFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Crea una instancia de operaciones para carpetas
    /// </summary>
    public CarpetaOperations CrearCarpetaOperations()
    {
        return _serviceProvider.GetRequiredService<CarpetaOperations>();
    }

    /// <summary>
    /// Crea una instancia de operaciones para solicitudes
    /// </summary>
    public SolicitudOperations CrearSolicitudOperations()
    {
        return _serviceProvider.GetRequiredService<SolicitudOperations>();
    }

    /// <summary>
    /// Crea una instancia de operaciones para PDF
    /// </summary>
    public PdfOperations CrearPdfOperations()
    {
        return _serviceProvider.GetRequiredService<PdfOperations>();
    }
}