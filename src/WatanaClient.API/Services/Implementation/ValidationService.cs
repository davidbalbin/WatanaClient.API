using Microsoft.Extensions.Logging;
using WatanaClient.API.Exceptions;
using WatanaClient.API.Models.Common;
using WatanaClient.API.Services.Interfaces;

namespace WatanaClient.API.Services.Implementation;

/// <summary>
/// Implementación del servicio de validación para comprobar campos y valores obligatorios
/// en las operaciones de la API de Watana. Este servicio ayuda a garantizar la integridad
/// de los datos antes de realizar operaciones en el servidor.
/// </summary>
public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de ValidationService.
    /// </summary>
    /// <param name="logger">Logger para registrar errores de validación.</param>
    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Valida que los campos especificados existan y no sean nulos en un objeto WatanaApiObject.
    /// </summary>
    /// <param name="data">Objeto a validar.</param>
    /// <param name="fieldNames">Nombres de los campos que deben existir.</param>
    /// <exception cref="WatanaValidationException">Se lanza cuando algún campo requerido no existe o es nulo.</exception>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// var data = new WatanaApiObject();
    /// data["nombre"] = "Juan";
    /// data["email"] = "juan@ejemplo.com";
    ///
    /// validationService.ValidateMandatoryFields(data, "nombre", "email", "telefono");
    /// // Lanzará WatanaValidationException porque 'telefono' no existe
    /// </code>
    /// </example>
    public void ValidateMandatoryFields(WatanaApiObject data, params string[] fieldNames)
    {
        foreach (var fieldName in fieldNames)
        {
            if (!data.ContainsKey(fieldName) || data[fieldName] is null)
            {
                _logger.LogError("Campo obligatorio no encontrado: {FieldName}", fieldName);
                throw new WatanaValidationException($"El campo '{fieldName}' es obligatorio");
            }
        }
    }

    /// <summary>
    /// Valida que una cadena tenga contenido válido (no sea nula ni vacía).
    /// </summary>
    /// <param name="value">Valor a validar.</param>
    /// <param name="paramName">Nombre del parámetro para el mensaje de error.</param>
    /// <exception cref="WatanaValidationException">Se lanza cuando el valor es nulo o está vacío.</exception>
    /// <example>
    /// Ejemplo de uso:
    /// <code>
    /// string? codigo = null;
    /// validationService.ValidateNotNullOrEmpty(codigo, "codigo");
    /// // Lanzará WatanaValidationException: "El valor 'codigo' no puede ser nulo o vacío"
    ///
    /// string nombre = "";
    /// validationService.ValidateNotNullOrEmpty(nombre, "nombre");
    /// // Lanzará WatanaValidationException: "El valor 'nombre' no puede ser nulo o vacío"
    /// </code>
    /// </example>
    public void ValidateNotNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
        {
            _logger.LogError("Valor requerido no proporcionado: {ParamName}", paramName);
            throw new WatanaValidationException($"El valor '{paramName}' no puede ser nulo o vacío");
        }
    }
}