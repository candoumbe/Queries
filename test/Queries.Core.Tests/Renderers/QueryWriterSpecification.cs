
using Bogus;

using FsCheck;

using Queries.Core.Renderers;

using System;

namespace Queries.Core.Tests.Renderers
{
    public class QueryWriterSpecification : ICommandGenerator<QueryWriter, QueryWriterState>
    {
        private readonly int _blockLevel;
        private readonly bool _prettyPrint;
        private readonly Faker _faker;

        public QueryWriterSpecification(int blockLevel, bool prettyPrint)
        {
            _blockLevel = blockLevel;
            _prettyPrint = prettyPrint;
            _faker = new Faker();
        }

        public QueryWriter InitialActual => new(_blockLevel, _prettyPrint);

        public QueryWriterState InitialModel => new(string.Empty, _blockLevel, _prettyPrint);

        public Gen<Command<QueryWriter, QueryWriterState>> Next(QueryWriterState nextValue)
        {
            return Gen.Elements<Command<QueryWriter, QueryWriterState>>(
                                                                        //new StartBlock(_faker.PickRandom("(", null)),
                                                                        //                                                        new EndBlock(_faker.PickRandom(")", null)),
                                                                        new WriteText(_faker.Lorem.Word()));
        }

        /// <summary>
        /// Command to run when starting a new block
        /// </summary>
        /// <remarks>
        /// This method is called by the framework to emulate a call to <see cref="QueryWriter.StartBlock(string)"/>
        /// </remarks>
        private class StartBlock : QueryWriterCommand
        {
            private readonly string _value;

            public StartBlock(string value)
            {
                _value = value;
            }

            ///<inheritdoc/>
            public override QueryWriter RunActual(QueryWriter value)
            {
                value.StartBlock(_value);
                return value;
            }

            ///<inheritdoc/>
            public override QueryWriterState RunModel(QueryWriterState model)
            {
                int newBlockLevel = model.BlockLevel + 1;
                int indentSize = _value is null
                    ? 0
                    : QueryWriter.IndentBlockSize * model.BlockLevel;

                return (initialValue: model.Value, prettyPrint: model.PrettyPrint, _value) switch
                {

                    ({ Length: 0 } or null, _, { Length: 0 } or null) =>
#if NETCOREAPP3_1
                                                                         new(value: string.Empty,
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                        model with
                                                                        {
                                                                            BlockLevel = newBlockLevel
                                                                        },
#endif

                    ({ Length: > 0 }, _, { Length: 0 } or null) =>
#if NETCOREAPP3_1
                                                                         new(value: model.Value,
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                         model with
                                                                         {
                                                                             BlockLevel = newBlockLevel
                                                                         },
#endif

                    ({ Length: 0 } or null, true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                    new(value: _value,
                                                                        blockLevel: newBlockLevel,
                                                                        prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                         model with
                                                                         {
                                                                             Value = $"{string.Empty.PadLeft(indentSize)}{_value}",
                                                                             BlockLevel = newBlockLevel
                                                                         },
#endif

                    (initialValue: { Length: > 0 }, prettyPrint: true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                         new(value: $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                         model with
                                                                         {
                                                                             Value = $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                                             BlockLevel = newBlockLevel
                                                                         },
#endif
                    (initialValue: { Length: > 0 }, prettyPrint: false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                         new(value: $"{model.Value} {_value}",
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                         model with
                                                                         {
                                                                             Value = $"{model.Value} {_value}",
                                                                             BlockLevel = newBlockLevel
                                                                         },
#endif

                    (_, false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                     new(value: _value,
                                                         blockLevel: newBlockLevel,
                                                         prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER                                         
                                                     model with
                                                     {
                                                         Value = _value,
                                                         BlockLevel = newBlockLevel
                                                     },
#endif
                };
            }

            ///<inheritdoc/>
            public override string ToString() => $"{nameof(StartBlock)}({_value})";
        }

        private class EndBlock : QueryWriterCommand
        {
            private readonly string _value;

            public EndBlock(string value)
            {
                _value = value;
            }

            ///<inheritdoc/>
            public override bool Pre(QueryWriterState actual) => actual.BlockLevel > 0 && actual.Value.Length == 0;

            ///<inheritdoc/>
            public override QueryWriter RunActual(QueryWriter value)
            {
                value.EndBlock(_value);
                return value;
            }

            ///<inheritdoc/>
            public override QueryWriterState RunModel(QueryWriterState model)
            {
                int newBlockLevel = model.BlockLevel - 1;
                int indentSize = _value is null
                    ? 0
                    : QueryWriter.IndentBlockSize * newBlockLevel;

                return (model.Value, model.PrettyPrint, _value) switch
                {
                    ({ Length: >= 0 }, _, { Length: 0 } or null) =>
#if NETCOREAPP3_1
                                                                    new(value: model.Value,
                                                                        blockLevel: newBlockLevel,
                                                                        prettyPrint: model.PrettyPrint),
#elif NET5_0_OR_GREATER
                                                                    model with { BlockLevel = newBlockLevel },
#endif

                    ({ Length: > 0 }, true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                     new(value: $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                         blockLevel: newBlockLevel,
                                                         prettyPrint: true),
#elif NET5_0_OR_GREATER
                                                     model with
                                                     {
                                                         Value = $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                         BlockLevel = newBlockLevel
                                                     },
#endif

                    ({ Length: 0 } or null, true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                        new(value: $"{string.Empty.PadLeft(indentSize)}{_value}",
                                                                            blockLevel: newBlockLevel,
                                                                            prettyPrint: true),
#elif NET5_0_OR_GREATER
                                                                        model with
                                                                        {
                                                                            Value = $"{string.Empty.PadLeft(indentSize)}{_value}",
                                                                            BlockLevel = newBlockLevel
                                                                        },
#endif

                    ({ Length: > 0 }, false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                    new(value: $"{model.Value} {_value}",
                                                        blockLevel: newBlockLevel,
                                                        prettyPrint: false),
#elif NET5_0_OR_GREATER
                                                    model with
                                                    {
                                                        Value = $"{model.Value} {_value}",
                                                        BlockLevel = newBlockLevel
                                                    },
#endif

                    ({ Length: 0 } or null, false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                        new(value: _value,
                                                                            blockLevel: newBlockLevel,
                                                                            prettyPrint: false),
#elif NET5_0_OR_GREATER
                                                                        model with
                                                                        {
                                                                            Value = $"{_value}",
                                                                            BlockLevel = newBlockLevel
                                                                        },
#endif

#if NETCOREAPP3_1
                    _ => new(value: string.Empty,
                             blockLevel: newBlockLevel,
                             prettyPrint: model.PrettyPrint)
#elif NET5_0_OR_GREATER
                    _ => model with { Value = string.Empty, BlockLevel = newBlockLevel },
#endif
                };
            }

            ///<inheritdoc/>
            public override string ToString() => $"{nameof(EndBlock)}({_value})";
        }

        /// <summary>
        /// Command to emulate a call to <see cref="QueryWriter.WriteText(string)"/>
        /// </summary>
        private class WriteText : QueryWriterCommand
        {
            private readonly string _value;

            public WriteText(string value)
            {
                _value = value;
            }

            ///<inheritdoc/>
            public override QueryWriter RunActual(QueryWriter value)
            {
                value.WriteText(_value);
                return value;
            }

            ///<inheritdoc/>
            public override QueryWriterState RunModel(QueryWriterState model)
            {
                int indentSize = (QueryWriter.IndentBlockSize * model.BlockLevel);

                return (model.Value, model.PrettyPrint, _value) switch
                {
                    ({ Length: > 0 } initialValue, false, { Length: > 0 } currentValue) => initialValue[^1] switch
                    {
                        '(' => new QueryWriterState($"{initialValue}{currentValue}", model.BlockLevel, false),
                        _ => new QueryWriterState($"{initialValue} {currentValue}", model.BlockLevel, false)
                    },
                    ({ Length: > 0 } initialValue, true, { Length: > 0 } currentValue) => new QueryWriterState($"{initialValue}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{currentValue}", model.BlockLevel, true),
                    ({ Length: 0 }, true, { Length: > 0 } currentValue) => new QueryWriterState($"{string.Empty.PadLeft(indentSize)}{currentValue}", model.BlockLevel, true),
                    ({ Length: 0 }, false, { Length: > 0 } currentValue) => new QueryWriterState(currentValue, model.BlockLevel, false),

                    _ => new QueryWriterState(model.Value, model.BlockLevel, model.PrettyPrint)
                };
            }

            ///<inheritdoc/>
            public override string ToString() => $"{nameof(WriteText)}({_value})";
        }

        /// <summary>
        /// Base class for writing <see cref="QueryWriter"/> commands for FsCheck.
        /// </summary>
        private abstract class QueryWriterCommand : Command<QueryWriter, QueryWriterState>
        {
            ///<inheritdoc/>
            public override Property Post(QueryWriter model, QueryWriterState expected)
            {
                (string value, int blockLevel, bool prettyPrint) current = (model.Value, model.BlockLevel, model.PrettyPrint);

                return (current == (expected.Value, expected.BlockLevel, expected.PrettyPrint)).ToProperty()
                    .Label($"{Environment.NewLine}Current  : ('{current.value.Replace(" ", "<space>").Replace(Environment.NewLine, "<newline>")}', blockLevel : {current.blockLevel})" +
                           $"{Environment.NewLine}expected : ('{expected.Value.Replace(" ", "<space>").Replace(Environment.NewLine, "<newline>")}', blockLevel : {expected.BlockLevel})");
            }
        }
    }
}
