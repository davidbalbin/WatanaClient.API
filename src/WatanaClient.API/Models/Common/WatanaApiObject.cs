using System.Text.Json.Serialization;
using WatanaClient.API.Exceptions;

namespace WatanaClient.API.Models.Common;

/// <summary>
/// Clase base para objetos de la API de Watana.
/// Implementa un diccionario dinámico para manejar las propiedades de los objetos de la API.
/// </summary>
public class WatanaApiObject : Dictionary<string, object>
{
    /// <summary>
    /// Obtiene o establece el tipo de operación a realizar.
    /// Este campo es ignorado durante la serialización JSON.
    /// </summary>
    /// <exception cref="ArgumentNullException">Se lanza cuando se intenta establecer un valor nulo.</exception>
    [JsonIgnore]
    public string? Operacion {
        get => ContainsKey("operacion") ? this["operacion"]?.ToString() : null;
        set => this["operacion"] = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaApiObject"/>.
    /// </summary>
    public WatanaApiObject() { }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WatanaApiObject"/> con una operación específica.
    /// </summary>
    /// <param name="operacion">El tipo de operación a realizar.</param>
    public WatanaApiObject(string operacion)
    {
        Operacion = operacion;
    }
    
    /// <summary>
    /// Valida si un campo específico existe y no es nulo.
    /// </summary>
    /// <param name="nombre">El nombre del campo a validar.</param>
    /// <exception cref="WatanaValidationException">Se lanza cuando el campo no existe o es nulo.</exception>
    public void ValidarCampoObligatorio(string nombre)
    {
        if (!ContainsKey(nombre) || this[nombre] is null)
        {
            throw new WatanaValidationException($"El campo '{nombre}' es obligatorio");
        }
    }
}