namespace Queries.Core.Renderers;

/// <summary>
/// <para>
/// Base class to derive from when creating a custom way to render <see cref="Parts.Columns.FieldColumn"/>'s<see cref="Parts.Columns.FieldColumn.Name"/> names.
/// </para>
/// </summary>
/// <remarks>
/// 
/// </remarks>
public abstract class FieldnameCasingStrategy
{
    /// <summary>
    /// A casing strategy that leave the fieldname casing unchanged
    /// </summary>
    public readonly static FieldnameCasingStrategy Default = new DefaultFieldnameCasingStrategy();

    /// <summary>
    /// A casing strategy that change each fieldname case to conform to camelCasing.
    /// </summary>
    public readonly static FieldnameCasingStrategy CamelCase = new CamelCaseCasingStrategy();

    /// <summary>
    /// A casing strategy that change each fieldName case to conform to snake casing
    /// </summary>
    public readonly static FieldnameCasingStrategy SnakeCase = new SnakeCaseCasingStrategy();

    /// <summary>
    /// Performs the desired transformation.
    /// The result of this method will be used instead of the <paramref name="fieldName"/>
    /// </summary>
    /// <param name="fieldName">The current</param>
    /// <returns>The desired fieldName</returns>
    public abstract string Handle(string fieldName);
}
