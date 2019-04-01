using System;

namespace Queries.Core.Renderers
{

    /// <summary>
    /// Kind of pagination supported by the renderer
    /// </summary>
    [Flags]
    public enum PaginationKind
    {
        None = 0x0,
        Top = 0x1,
        Limit = 0x2,
    }
}