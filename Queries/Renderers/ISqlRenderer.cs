using Queries.Builders;

namespace Queries.Renderers
{
    public interface ISqlRenderer
    {
        /// <summary>
        /// Renders the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        string Render(SelectQueryBase query);

        /// <summary>
        /// Renders the specified update query.
        /// </summary>
        /// <param name="query">The update query.</param>
        /// <returns></returns>
        string Render(UpdateQuery query);

        /// <summary>
        /// Renders the specified delete query.
        /// </summary>
        /// <param name="query">The delete query.</param>
        /// <returns></returns>
        string Render(DeleteQuery query);

        /// <summary>
        /// Renders the specified query to create a view.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        string Render(CreateViewQuery query);

        /// <summary>
        /// Renders the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        string Render(TruncateQuery query);
    }
}
