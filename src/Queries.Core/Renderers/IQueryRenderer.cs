
namespace Queries.Core.Renderers
{
    /// <summary>
    /// The extension point to implement to provide conversion from <see cref="IQuery"/> to its a database equivalent <see cref="string"/>
    /// </summary>
    public interface IQueryRenderer
    {
        /// <summary>
        /// Converts the specified <see cref="IQuery"/> to its provider
        /// </summary>
        /// <param name="query">The query to render</param>
        /// <returns>The specific string</returns>
        string Render(IQuery query);
    }
}

