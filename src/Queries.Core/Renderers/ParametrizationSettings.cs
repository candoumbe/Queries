namespace Queries.Core.Renderers
{
    public enum ParametrizationSettings
    {
        /// <summary>
        /// Uses this settings to render both variable declaration section and use variable in the statement
        /// </summary>
        Default = 0,

        /// <summary>
        /// Replace values with their respective parameters and skip parameters déclaration
        /// </summary>
        SkipVariableDeclaration = 1,

        /// <summary>
        /// Do not create variable
        /// </summary>
        None = 2
    }
}