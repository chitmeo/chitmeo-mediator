namespace ChitMeo.Mediator;

/// <summary>
/// Configuration options for the mediator.
/// </summary>
public sealed class MediatorOptions
{
    /// <summary>
    /// The substring used to identify module assemblies to scan for handlers.
    /// Defaults to ".Module".
    /// </summary>
    public string ModulePrefix { get; set; } = ".Module";
}
