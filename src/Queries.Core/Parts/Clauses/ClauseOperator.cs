namespace Queries.Core.Parts.Clauses;

/// <summary>
/// Operators that can be apply when building a <see cref="WhereClause"/>
/// </summary>
public enum ClauseOperator
{
    /// <summary>
    /// The <c>=</c> operator
    /// </summary>
    EqualTo,

    /// <summary>
    /// The <c>!=</c> operator
    /// </summary>
    NotEqualTo,

    /// <summary>
    /// The <c>&lt;</c> operator
    /// </summary>
    LessThan,

    /// <summary>
    /// The <c>&gt;</c> operator
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The <c>&lt;=</c> operator
    /// </summary>
    LessThanOrEqualTo,

    /// <summary>
    /// The <c>&gt;=</c> operator
    /// </summary>
    GreaterThanOrEqualTo,
    
    /// <summary>
    /// The like operator
    /// </summary>
    Like,

    /// <summary>
    /// The not like operator
    /// </summary>
    NotLike,

    /// <summary>
    /// Matches operator that can check wheter a value does not exist
    /// </summary>
    IsNull,

    /// <summary>
    /// Use this operator to make a <see cref="IClause{T}"/> checks if the <see cref="IClause{T}.Column"/> has a value that is not
    /// </summary>
    IsNotNull,

    /// <summary>
    /// Use this operator to make a <see cref="IClause{T}"/> checks if the <see cref="IClause{TColumn}"/> matches at least one value of a set of data
    /// </summary>
    In,

    /// <summary>
    /// Use this operator to make a <see cref="IClause{T}"/> checks if the <see cref="IClause{TColumn}"/> does not match at least one value of a set of data
    /// </summary>
    NotIn
}