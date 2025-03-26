using WatanaClient.API.Models.Common;
using WatanaClient.API.Exceptions;

namespace WatanaClient.API.Services.Interfaces;

/// <summary>
/// Servicio para validación de datos y parámetros en las operaciones de la API.
/// Esta interfaz proporciona métodos para validar la integridad y completitud de los datos
/// antes de ser enviados al servidor.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Valida que los campos especificados existan y no sean nulos en un objeto WatanaApiObject.
    /// </summary>
    /// <param name="data">Objeto WatanaApiObject a validar.</param>
    /// <param name="fieldNames">Nombres de los campos que deben estar presentes y no ser nulos.</param>
    /// <exception cref="WatanaValidationException">Se lanza cuando algún campo requerido no existe o es nulo.</exception>
    /// <remarks>
    /// Este método es útil para validar que todos los campos necesarios estén presentes antes de realizar
    /// una operación en la API. Por ejemplo, al crear una carpeta o enviar una solicitud de firma.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var data = new WatanaApiObject();
    /// data["carpeta_codigo"] = "CARP001";
    /// data["firmante"] = new { nombre = "Juan", email = "juan@ejemplo.com" };
    ///
    /// validationService.ValidateMandatoryFields(data, "carpeta_codigo", "firmante");
    /// </code>
    /// </example>
    void ValidateMandatoryFields(WatanaApiObject data, params string[] fieldNames);
    
    /// <summary>
    /// Valida que una cadena tenga contenido válido (no sea nula ni vacía).
    /// </summary>
    /// <param name="value">Valor de la cadena a validar.</param>
    /// <param name="paramName">Nombre del parámetro para incluir en el mensaje de error.</param>
    /// <exception cref="WatanaValidationException">Se lanza cuando el valor es nulo o está vacío.</exception>
    /// <remarks>
    /// Este método es útil para validar parámetros de entrada como códigos de carpeta,
    /// identificadores y otros campos de texto que no deben estar vacíos.
    /// </remarks>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// string carpetaCodigo = "";
    /// validationService.ValidateNotNullOrEmpty(carpetaCodigo, nameof(carpetaCodigo));
    /// // Lanzará WatanaValidationException: "El campo 'carpetaCodigo' es obligatorio"
    /// </code>
    /// </example>
    void ValidateNotNullOrEmpty(string? value, string paramName);
}