
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
            return Gen.Elements<Command<QueryWriter, QueryWriterState>>(new StartBlock(_faker.PickRandom("(", null)),
                                                                        new EndBlock(_faker.PickRandom(")", null)),
                                                                        new WriteLine(_faker.Lorem.Word()));
        }

        private class StartBlock : QueryWriterCommand
        {
            private readonly string _value;

            public StartBlock(string value)
            {
                _value = value;
            }

            public override QueryWriter RunActual(QueryWriter value)
            {
                value.StartBlock(_value);
                return value;
            }

            public override QueryWriterState RunModel(QueryWriterState model)
            {
                int newBlockLevel = model.BlockLevel + 1;
                int indentSize = _value is null
                    ? 0
                    : QueryWriter.IndentBlockSize * model.BlockLevel;

                return (initialValue : model.Value, prettyPrint: model.PrettyPrint, _value) switch
                {
                    ({ Length: >= 0 } or null, _, { Length: 0 } or null) =>
#if NETCOREAPP3_1
                                                                         new(value: model.Value,
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0
                                                                         model with
                                                                         {
                                                                            BlockLevel = newBlockLevel
                                                                         },
#else
#error Unsupported framwork
#endif

                    (initialValue: { Length: > 0 } or null, prettyPrint: true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                         new(value: $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0
                                                                         model with
                                                                         {
                                                                            Value = $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                                            BlockLevel = newBlockLevel
                                                                         },
#else
#error Unsupported framwork
#endif
                    (initialValue: { Length: > 0 } or null, prettyPrint: false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                         new(value: $"{model.Value} {_value}",
                                                                             blockLevel: newBlockLevel,
                                                                             prettyPrint: model.PrettyPrint),
#elif NET5_0
                                                                         model with
                                                                         {
                                                                            Value = $"{model.Value} {_value}",
                                                                            BlockLevel = newBlockLevel
                                                                         },
#else
#error Unsupported framwork
#endif
                    ({ Length: > 0 }, _, _) =>
#if NETCOREAPP3_1
                                                   new(value: model.Value,
                                                       blockLevel: newBlockLevel,
                                                       prettyPrint: model.PrettyPrint),
#elif NET5_0
                                                   model with
                                                   {
                                                      Value = _value,
                                                      BlockLevel = newBlockLevel
                                                   },
#else
#error Unsupported framwork
#endif

                    (_, true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                     new(value: $"{string.Empty.PadLeft(indentSize)}{_value}",
                                                         blockLevel: newBlockLevel,
                                                         prettyPrint: model.PrettyPrint),
#elif NET5_0                                         
                                                     model with
                                                     {
                                                        Value = _value,
                                                        BlockLevel = newBlockLevel
                                                     },
#else
#error Unsupported framwork
#endif
                    (_, false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                     new(value: _value,
                                                         blockLevel: newBlockLevel,
                                                         prettyPrint: model.PrettyPrint),
#elif NET5_0                                         
                                                     model with
                                                     {
                                                        Value = _value,
                                                        BlockLevel = newBlockLevel
                                                     },
#else
#error Unsupported framwork
#endif

                    _ =>
#if NETCOREAPP3_1
                          new(value: string.Empty,
                              blockLevel: newBlockLevel,
                              prettyPrint: model.PrettyPrint),
#elif NET5_0
                          model with
                          {
                             Value = string.Empty,
                             BlockLevel = newBlockLevel
                          }
#else
#error Unsupported framwork
#endif
                };
            }

            public override string ToString() => $"{nameof(StartBlock)}({_value})";
        }

        private class EndBlock : QueryWriterCommand
        {
            private readonly string _value;

            public EndBlock(string value)
            {
                _value = value;
            }

            public override bool Pre(QueryWriterState actual) => actual.BlockLevel > 0 && actual.Value.Length == 0;

            public override QueryWriter RunActual(QueryWriter value)
            {
                value.EndBlock(_value);
                return value;
            }

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
#elif NET5_0
                                                                    model with { BlockLevel = newBlockLevel },
#else
#error Unsupported framwork
#endif

                    ({ Length: > 0 }, true, { Length : > 0}) =>
#if NETCOREAPP3_1
                                                     new(value: $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                         blockLevel: newBlockLevel,
                                                         prettyPrint: true),
#elif NET5_0
                                                     model with
                                                     {
                                                        Value = $"{model.Value}{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                        BlockLevel = newBlockLevel
                                                     },
#else
#error Unsupported framwork
#endif

                    ({ Length: 0 } or null, true, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                        new(value: $"{string.Empty.PadLeft(indentSize)}{_value}",
                                                                            blockLevel: newBlockLevel,
                                                                            prettyPrint: true),
#elif NET5_0
                                                                        model with
                                                                        {
                                                                            Value = $"{Environment.NewLine}{string.Empty.PadLeft(indentSize)}{_value}",
                                                                            BlockLevel = newBlockLevel
                                                                        },
#else
#error Unsupported framework
#endif

                    ({ Length: > 0 }, false, { Length : > 0 }) =>
#if NETCOREAPP3_1
                                                    new(value: $"{model.Value} {_value}",
                                                        blockLevel: newBlockLevel,
                                                        prettyPrint: false),
#elif NET5_0
                                                    model with
                                                    {
                                                        Value =  $"{model.Value} {_value}",
                                                        BlockLevel = newBlockLevel
                                                    },
#else
#error Unsupported framework
#endif

                    ({ Length: 0 } or null, false, { Length: > 0 }) =>
#if NETCOREAPP3_1
                                                                        new(value: _value,
                                                                            blockLevel: newBlockLevel,
                                                                            prettyPrint: false),
#elif NET5_0
                                                                        model with
                                                                        {
                                                                            Value =  $"{model.Value} {_value}",
                                                                            BlockLevel = newBlockLevel
                                                                        },
#else
#error Unsupported framework
#endif

#if NETCOREAPP3_1
                    _ => new(value: string.Empty,
                             blockLevel: newBlockLevel,
                             prettyPrint: model.PrettyPrint)
#elif NET5_0
                    _ => model with { Value = string.Empty, BlockLevel = newBlockLevel },
#else
#error Unsupported framework 
#endif
                };
            }

            public override string ToString() => $"{nameof(EndBlock)}({_value})";
        }

        private class WriteLine : QueryWriterCommand
        {
            private readonly string _value;

            public WriteLine(string value)
            {
                _value = value;
            }

            public override QueryWriter RunActual(QueryWriter value)
            {
                value.WriteLine(_value);
                return value;
            }

            public override QueryWriterState RunModel(QueryWriterState model)
            {
                int indentSize = (QueryWriter.IndentBlockSize * model.BlockLevel);
                bool isEmpty = model.Value.Length == 0;
                string newValue = model.PrettyPrint
                                    ? $"{model.Value}{Environment.NewLine : string.Empty)}{string.Empty.PadLeft(indentSize)}{_value}"
                                    : $"{model.Value}{Environment.NewLine}{_value}";

                return (model.Value, model.PrettyPrint, _value) switch
                {
                    ({ Length: > 0 }, true, _ ) =>
#if NETCOREAPP3_1
                                                new(value: $"{model.Value}{Environment.NewLine}{(model.PrettyPrint ? string.Empty.PadLeft(indentSize) : string.Empty)}{_value}",
                                                    blockLevel : model.BlockLevel,
                                                    prettyPrint: model.PrettyPrint
                                                ),
#elif NET5_0
                                                model with
                                                {
                                                    Value = $"{model.Value}{(model.PrettyPrint ? string.Empty.PadLeft(indentSize): string.Empty)}{_value}"
                                                },
#else
#error Unsupported framework
#endif

                    ({ Length: > 0 }, false, _) =>
#if NETCOREAPP3_1
                                                new(value: $"{model.Value}{Environment.NewLine}{_value}",
                                                    blockLevel: model.BlockLevel,
                                                    prettyPrint: model.PrettyPrint
                                                ),
#elif NET5_0
                                                model with
                                                {
                                                    Value = $"{model.Value}{Environment.NewLine}{_value}"
                                                },
#else
#error Unsupported framework
#endif

                   _ =>
#if NETCOREAPP3_1
                                                     new(value: $"{(model.PrettyPrint ? string.Empty.PadLeft(indentSize) : string.Empty)}{_value}",
                                                         blockLevel: model.BlockLevel,
                                                         prettyPrint: model.PrettyPrint
                                                     ),
#elif NET5_0
                                                     model with
                                                     {
                                                         Value = $"{(model.PrettyPrint ? string.Empty.PadLeft(indentSize) : string.Empty)}{_value}"
                                                     },
#else
#error Unsupported framework
#endif

                };
            }

            public override string ToString() => $"{nameof(WriteLine)}({_value})";
        }

        /// <summary>
        /// Base class for writing <see cref="QueryWriter"/> commands for FsCheck.
        /// </summary>
        private abstract class QueryWriterCommand : Command<QueryWriter, QueryWriterState>
        {
            public override Property Post(QueryWriter model, QueryWriterState expected)
            {
                (string value, int blockLevel, bool prettyPrint) current = (model.Value, model.BlockLevel, model.PrettyPrint);

                return (current == (expected.Value, expected.BlockLevel, expected.PrettyPrint)).ToProperty()
                    .Label($"{Environment.NewLine}Current  : ('{ current.value.Replace(' ', '*')}', blockLevel : { current.blockLevel})" +
                           $"{Environment.NewLine}expected : ('{expected.Value.Replace(' ', '*')}', blockLevel : {expected.BlockLevel})");
            }
        }
    }
}
