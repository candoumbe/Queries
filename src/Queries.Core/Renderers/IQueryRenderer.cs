namespace Queries.Core.Renderers;

/// <summary>
/// The extension point to implement to provide conversion from <see cref="IQuery"/> to its a database equivalent <see cref="string"/>
/// </summary>
public interface IQueryRenderer
{
    /// <summary>
    /// Converts the specified <see cref="IQuery"/> to a <see langword="string"/> that is suitable to interact with a specific store engine.
    /// </summary>
    /// <param name="query">The query to render</param>
    /// <returns>The specific <see langword="string"/></returns>
    string Render(IQuery query);

    /// <summary>
    /// Performs various operations to gather informations on the current <paramref name="query"/>
    /// </summary>
    /// <param name="query">The query to compile</param>
    /// <returns>a <see cref="CompiledQuery"/> instance</returns>
    /// <exception cref="System.ArgumentNullException">if <paramref name="query"/> is <see langword="null" />.</exception>
    CompiledQuery Compile(IQuery query);
}

