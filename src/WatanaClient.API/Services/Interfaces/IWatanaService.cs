using WatanaClient.API.Models.Common;

namespace WatanaClient.API.Services.Interfaces;

/// <summary>
/// Define las operaciones base para la comunicación con la API de Watana.
/// Esta interfaz proporciona los métodos fundamentales para enviar solicitudes al servidor.
/// </summary>
public interface IWatanaService
{
    /// <summary>
    /// Envía una solicitud a la API y deserializa la respuesta al tipo especificado.
    /// </summary>
    /// <typeparam name="T">El tipo de objeto esperado en la respuesta.</typeparam>
    /// <param name="operacion">El tipo de operación a realizar en la API.</param>
    /// <param name="data">Los datos de la solicitud encapsulados en un objeto <see cref="WatanaApiObject"/>.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>La respuesta deserializada de tipo T.</returns>
    /// <remarks>
    /// Este método se utiliza cuando se espera una respuesta estructurada que puede ser mapeada
    /// a un tipo específico, como <see cref="Models.Responses.CarpetaResponse"/> o
    /// <see cref="Models.Responses.SolicitudResponse"/>.
    /// </remarks>
    Task<T> EnviarSolicitudAsync<T>(string operacion, WatanaApiObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía una solicitud a la API y retorna la respuesta como una cadena JSON sin procesar.
    /// </summary>
    /// <param name="operacion">El tipo de operación a realizar en la API.</param>
    /// <param name="data">Los datos de la solicitud encapsulados en un objeto <see cref="WatanaApiObject"/>.</param>
    /// <param name="cancellationToken">Token para cancelar la operación de forma asincrónica.</param>
    /// <returns>La respuesta JSON como una cadena de texto.</returns>
    /// <remarks>
    /// Este método es útil cuando:
    /// - Se necesita acceder a la respuesta JSON sin procesar
    /// - La estructura de la respuesta es dinámica o no se conoce de antemano
    /// - Se requiere un procesamiento personalizado de la respuesta
    /// </remarks>
    Task<string> EnviarSolicitudRawAsync(string operacion, WatanaApiObject data, CancellationToken cancellationToken = default);
}