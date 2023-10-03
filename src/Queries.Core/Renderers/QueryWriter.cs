
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// A custom implementation of `StringWriter` that provides functionality for writing and 
    /// formatting text with indentation and block levels. It supports pretty printing and allows for starting and ending blocks of text.
    /// </summary>
    /// <example>
    /// <code>
    /// QueryWriter writer = new QueryWriter();
    /// writer.StartBlock("if (condition)");
    /// writer.WriteText("Console.WriteLine(\"Condition is true!\");");
    /// writer.EndBlock();
    /// writer.WriteText("Console.WriteLine(\"End of program.\");");
    /// string result = writer.ToString();
    /// Console.WriteLine(result);
    /// </code>
    /// Output:
    /// <code>
    /// if (condition)
    /// {
    ///     Console.WriteLine("Condition is true!");
    /// }
    /// Console.WriteLine("End of program.");
    /// </code>
    /// </example>
    /// <remarks>
    /// Main functionalities:
    /// - Provides indentation and block level handling for writing text.
    /// - Supports pretty printing with customizable indentation size.
    /// - Allows for starting and ending blocks of text.
    /// - Handles newline and spacing formatting based on the current block level.
    /// </remarks>
    /// <seealso cref="StringWriter"/>
    public class QueryWriter
    {
        /// <summary>
        /// Gets the current block level
        /// </summary>
        public int BlockLevel
        {
            get => _blockLevel;

            private set
            {
                MustPrintIndentation = value != BlockLevel && PrettyPrint;
                _blockLevel = value;
            }
        }

        private int _blockLevel;

        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// Gets the current text so far
        /// </summary>
        public string Value => _stringBuilder.ToString();

        /// <summary>
        /// Indentation size to delimit a block
        /// </summary>
        public const int IndentBlockSize = 4;
        private const char SpaceChar = ' ';

        public int Length => _stringBuilder.Length;

        private bool MustPrintIndentation { get; set; }

        private bool _mustPrintNewLine;

        private bool MustPrintNewLine
        {
            get => _mustPrintNewLine;
            set
            {
                _mustPrintNewLine = value;
                MustPrintIndentation = value || (PrettyPrint && _stringBuilder.Length == 0);
            }
        }

        /// <summary>
        /// Indicates how to render the underlying value
        /// </summary>
        public bool PrettyPrint { get; }

        /// <summary>
        /// Creates a new instance of <see cref="QueryWriter"/>
        /// </summary>
        /// <param name="blockLevel"></param>
        /// <param name="prettyPrint"></param>
        public QueryWriter(int blockLevel = 0, bool prettyPrint = true)
        {
            if (blockLevel < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockLevel), blockLevel, "Cannot be negative value");
            }
            BlockLevel = blockLevel;
            PrettyPrint = prettyPrint;
            MustPrintIndentation = prettyPrint && blockLevel > 0;
            _stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Starts a new "block" and increment the <see cref="BlockLevel"/>.
        /// </summary>
        /// <param name="value">The value to write BEFORE starting a new indentation</param>
        /// <remarks>
        ///    When <see cref="PrettyPrint"/> is <see langword="true"/>, any subsequentx <see cref="WriteText(string)"/> operation will be indented according to the current value of <see cref="BlockLevel"/>
        ///    until explicitly calling <see cref="EndBlock"/>.
        /// </remarks>
        public void StartBlock(string value = null)
        {
            if (value is not null)
            {
                MustPrintNewLine = (_stringBuilder.Length > 0 && PrettyPrint);
                if (MustPrintNewLine)
                {
                    _stringBuilder.AppendLine();
                }
                else if (_stringBuilder.Length > 0)
                {
                    _stringBuilder.Append(SpaceChar);
                }
                _stringBuilder.Append(value);
                MustPrintNewLine = PrettyPrint;
            }

            BlockLevel++;
        }

        /// <summary>
        /// Ends a block previously opened by <see cref="StartBlock"/>.
        /// </summary>
        /// <remarks>This will be a no-op if <see cref="StartBlock"/> has not been called prior to calling this method</remarks>
        /// <param name="value">A value to write AFTER "ending" the current block</param>
        public void EndBlock(string value = null)
        {
            DecreaseBlockLevel();

            if (value is not null)
            {
                MustPrintNewLine = MustPrintNewLine || (PrettyPrint && _stringBuilder.Length > 0);

                if (MustPrintNewLine)
                {
                    _stringBuilder.AppendLine();
                }
                if (PrettyPrint)
                {
                    _stringBuilder.Append(new string(' ', IndentBlockSize * BlockLevel));
                }

                _stringBuilder.Append(value);
                MustPrintNewLine = PrettyPrint;
            }

            void DecreaseBlockLevel()
            {
                if (BlockLevel > 0)
                {
                    BlockLevel--;
                }
            }
        }

        /// <summary>
        /// Writes the specified text to the StringBuilder object, applying indentation and spacing based on the current block level and pretty print settings.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        public void WriteText(string text)
        {
            if (text is not null)
            {
                if (_stringBuilder.Length > 0)
                {
                    if (PrettyPrint)
                    {
                        _stringBuilder.AppendLine();
                    }
                    else
                    {
                        _ = (_stringBuilder[_stringBuilder.Length - 1], text) switch
                        {
                            ('(', _) => _stringBuilder,
                            (')', { Length: 1 }) _ => text[0] switch
                            {
                                ')' or ',' => _stringBuilder,
                                _ => _stringBuilder.Append(SpaceChar)
                            },
                            _ => _stringBuilder.Append(SpaceChar)
                        };
                    }
                }

                if (PrettyPrint && BlockLevel > 0)
                {
                    int indentationSize = BlockLevel * IndentBlockSize;
                    string padding = string.Empty.PadLeft(indentationSize);
#if NET6_0_OR_GREATER
                    text = string.Join(Environment.NewLine, text.Split(Environment.NewLine)
                                           .Select(part => $"{padding}{part}"));
#else
                    text = string.Join("\n", text.Split('\n')
                                           .Select(part => $"{padding}{part}"));

#endif
                    _stringBuilder.Append(text);
                }
                else
                {
                    _stringBuilder.Append(text);
                }
            }
        }

        ///<inheritdoc/>
        public override string ToString() => _stringBuilder.ToString();
    }
}