using Newtonsoft.Json;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// Settings to use when computing string representation of a <see cref="IQuery"/> instance.
    /// </summary>
    public class QueryRendererSettings
    {
        /// <summary>
        /// Defines how to print queries.
        /// </summary>
        /// <remarks>
        /// Gives a hint to a <see cref="QueryRendererBase"/> implementation on how to display
        /// </remarks>
        public bool PrettyPrint { get; set; }

        /// <summary>
        /// Sets how <see cref="System.DateTime"/>/<see cref="System.DateTimeOffset"/> should be printed
        /// </summary>
        public string DateFormatString { get; set; }

    }
}