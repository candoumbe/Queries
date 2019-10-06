﻿using System;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// Settings to use when computing string representation of a <see cref="IQuery"/> instance.
    /// </summary>
    public abstract class QueryRendererSettings
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

        /// <summary>
        /// Defines the kind of pagination supported by the renderer
        /// </summary>
        public PaginationKind PaginationKind { get; }

        /// <summary>
        /// Indicates that the renderer should not declare variables if any
        /// </summary>
        /// <remarks>
        /// Depending on the renderer implementation, this may or may not be fullfilled
        /// </remarks>
        public bool SkipVariableDeclaration { get; set; }

        public override string ToString() => this.Jsonify();

        protected QueryRendererSettings(PaginationKind paginationKind) => PaginationKind = paginationKind;
    }
}