using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;

namespace Queries.Core.Builders;

/// <summary>
/// Helps to build <see cref="Variable"/> instances.
/// </summary>
public class DeclareVariableQuery : IBuild<Variable>
{
    private readonly string _name;
    private VariableType _type;
    private object _value;

    /// <summary>
    /// Builds a new <see cref="DeclareVariableQuery"/> instance
    /// </summary>
    /// <param name="name">name of the variable</param>
    public DeclareVariableQuery(string name)
    {
        _name = name;
    }

    /// <summary>
    /// Defines the value of the <see cref="Variable"/> that the current <see cref="DeclareVariableQuery"/> 
    /// instance is building.
    /// </summary>
    /// <param name="value">the value of the parameter</param>
    /// <returns>the current <see cref="DeclareVariableQuery"/> instance</returns>
    public DeclareVariableQuery WithValue(object value)
    {
        _value = value;

        return this;
    }

    /// <summary>
    /// Set the type of <see cref="Variable"/> the current instance of <see cref="DeclareVariableQuery"/>
    /// is building as <see cref="VariableType.Numeric"/>.
    /// </summary>
    /// <returns></returns>
    public IBuild<Variable> Numeric()
    {
        _type = VariableType.Numeric;
        return this;
    }

    /// <summary>
    /// Set the type of <see cref="Variable"/> the current instance of <see cref="DeclareVariableQuery"/>
    /// is building as <see cref="VariableType.String"/>.
    /// </summary>
    /// <returns></returns>
    public IBuild<Variable> String()
    {
        _type = VariableType.String;
        return this;
    }

    /// <summary>
    /// Set the type of <see cref="Variable"/> the current instance of <see cref="DeclareVariableQuery"/>
    /// is building as <see cref="VariableType.Date"/>.
    /// </summary>
    /// <returns></returns>
    public IBuild<Variable> Date()
    {
        _type = VariableType.Date;
        return this;
    }

    ///<inheritdoc/>
    public Variable Build() => new(_name, _type, _value);
}
