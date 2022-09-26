using System;

namespace Queries.Core.Renderers;

/// <summary>
/// Kind of pagination supported by renderers
/// </summary>
[Flags]
public enum PaginationKind
{
    /// <summary>
    /// Indicates that the renderer does not support pagination
    /// </summary>
    None = 0x0,
    /// <summary>
    /// Renderer supports <see cref="Top"/> operator
    /// </summary>
    Top = 0x1,

    /// <summary>
    /// Renderer supports <see cref="Limit"/> operation
    /// </summary>
    Limit = 0x2,
}