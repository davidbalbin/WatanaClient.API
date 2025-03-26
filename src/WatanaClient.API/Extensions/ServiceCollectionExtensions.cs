using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatanaClient.API.Authentication;
using WatanaClient.API.Interfaces;
using WatanaClient.API.Operations;
using WatanaClient.API.Services.Implementation;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Extensions;

/// <summary>
/// Proporciona métodos de extensión para configurar los servicios del cliente Watana
/// en el contenedor de inyección de dependencias.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Agrega y configura los servicios del cliente Watana usando un archivo de configuración.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <param name="configuration">La configuración que contiene las opciones de autenticación.</param>
    /// <returns>La colección de servicios para permitir el encadenamiento.</returns>
    /// <remarks>
    /// Este método registra todos los servicios necesarios para el cliente Watana:
    /// - Configura las opciones de autenticación desde la sección de configuración
    /// - Registra los servicios base como singletons
    /// - Registra las operaciones específicas como singletons
    /// </remarks>
    public static IServiceCollection AddWatanaClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar opciones de autenticación
        services.Configure<AuthenticationOptions>(
            configuration.GetSection(AuthenticationOptions.ConfigurationSection));

        // Registrar servicios base
        services.AddHttpClient<IWatanaService, WatanaService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IWatanaClient, WatanaClient>();

        // Registrar operaciones
        services.AddSingleton<CarpetaOperations>();
        services.AddSingleton<SolicitudOperations>();
        services.AddSingleton<PdfOperations>();
        services.AddSingleton<WatanaFactory>();

        return services;
    }

    /// <summary>
    /// Agrega y configura los servicios del cliente Watana usando una acción de configuración.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <param name="configureOptions">La acción que configura las opciones de autenticación.</param>
    /// <returns>La colección de servicios para permitir el encadenamiento.</returns>
    /// <remarks>
    /// Este método es útil cuando se prefiere configurar las opciones programáticamente:
    /// - Configura las opciones de autenticación mediante una acción
    /// - Registra los mismos servicios base como singletons
    /// - Registra las mismas operaciones específicas como singletons
    /// </remarks>
    public static IServiceCollection AddWatanaClient(this IServiceCollection services, Action<AuthenticationOptions> configureOptions)
    {
        // Configurar opciones directamente
        services.Configure(configureOptions);

        // Registrar servicios base
        services.AddHttpClient<IWatanaService, WatanaService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IWatanaClient, WatanaClient>();

        // Registrar operaciones
        services.AddSingleton<CarpetaOperations>();
        services.AddSingleton<SolicitudOperations>();
        services.AddSingleton<PdfOperations>();
        services.AddSingleton<WatanaFactory>();

        return services;
    }
}