namespace Queries.Core.Parts;

/// <summary>
/// Marks an element that can be named
/// </summary>
public interface INamable
{
    /// <summary>
    /// Name associated 
    /// </summary>
    string Name { get; }
}
