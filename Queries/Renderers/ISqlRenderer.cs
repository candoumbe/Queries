using Queries.Builders;

namespace Queries.Renderers
{
    public interface ISqlRenderer
    {
        /// <summary>
        /// Renders the specified query.
        /// </summary>
        /// <param name="selectQuery">The query.</param>
        /// <returns></returns>
        string Render(SelectQueryBase selectQuery);

        /// <summary>
        /// Renders the specified update query.
        /// </summary>
        /// <param name="updateQuery">The update query.</param>
        /// <returns></returns>
        string Render(UpdateQuery updateQuery);
    }
}
