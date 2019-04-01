using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Tools
{
    public static class SystemExtensions
    {
        /// <summary>
        /// Performs a deep copy of <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source element</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T source) => DeserializeObject<T>(SerializeObject(source));
    }
}
