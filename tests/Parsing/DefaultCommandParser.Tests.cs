using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Finite.Commands.Parsing.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="DefaultCommandParser"/>.
    /// </summary>
    public class DefaultCommandParserTests
    {
        /// <summary>
        /// Ensures that <see cref="DefaultCommandParser.Lex(string)"/>
        /// produces the correct results.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(GetLexInputs))]
        public void LexReturnsCorrectlyTokenisedItems(LexInput input)
        {
            var result = DefaultCommandParser.Lex(input.Message);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count(),
                Is.EqualTo(input.ImportantTokens.Count()));

            foreach (var (expectedToken, actualToken) in
                input.ImportantTokens.Zip(result))
            {
                Assert.That(actualToken.HasValue, Is.True);
                var (offset, length) = actualToken.Portion.GetOffsetAndLength(
                    input.Message.Length);

                Assert.That(offset, Is.GreaterThanOrEqualTo(0));
                Assert.That(length, Is.GreaterThanOrEqualTo(0));
                Assert.That(length,
                    Is.LessThanOrEqualTo(input.Message.Length));

                Assert.That(actualToken.Portion, Is.EqualTo(expectedToken));
            }
        }

        /// <summary>
        /// An input for <see cref="DefaultCommandParser.Lex(string)"/>
        /// </summary>
        public struct LexInput
        {
            /// <summary>
            /// The raw message to pass to
            /// <see cref="DefaultCommandParser.Lex(string)"/>
            /// </summary>
            public string Message;

            /// <summary>
            /// The <see cref="Range"/>s to expect as output.
            /// </summary>
            public IEnumerable<Range> ImportantTokens;
        }

        private static IEnumerable<LexInput> GetLexInputs()
        {
            foreach (var group in GetAllInputs())
                foreach (var input in group)
                    yield return input;

            // TODO: Range calculation here should be simplified eventually

            static IEnumerable<IEnumerable<LexInput>> GetAllInputs()
            {
                yield return GetInputs("");
                yield return GetInputs("group ",
                    Index.Start .. Index.FromStart("group".Length)
                );
                yield return GetInputs("group subgroup ",
                    Index.Start .. Index.FromStart("group".Length),
                    Index.FromStart("group ".Length) .. Index.FromStart("group subgroup".Length)
                );
            }

            static IEnumerable<LexInput> GetInputs(string prefix,
                params Range[] ranges)
            {
                yield return new LexInput
                {
                    Message = $"{prefix}name",
                    ImportantTokens = ranges.Append(
                        Index.FromStart(prefix.Length) .. Index.End
                    )
                };

                foreach (var parameter1 in GetParameters(true))
                    foreach (var parameter2 in GetParameters(false))
                        yield return new LexInput
                        {
                            Message = $"{prefix}name{parameter1}{parameter2}",
                            ImportantTokens = GetRanges(ranges, prefix,
                                parameter1, parameter2)
                        };

                static IEnumerable<Range> GetRanges(IEnumerable<Range> ranges,
                    string prefix, string parameter1, string parameter2)
                {
                    foreach (var range in ranges)
                        yield return range;

                    yield return Index.FromStart(prefix.Length) .. Index.FromStart(prefix.Length + 4);

                    if (parameter1.Length > 0)
                        yield return Index.FromStart(prefix.Length + 5) .. (prefix.Length + 4 + parameter1.Length);

                    if (parameter2.Length > 0)
                        yield return Index.FromStart(prefix.Length + 5 + parameter1.Length) .. Index.End;
                }
            }

            static IEnumerable<string> GetParameters(bool includeEmpty)
            {
                if (includeEmpty)
                    yield return "";
                yield return " parameter";
                yield return " <#1234567890>";
                yield return " \"parameter with spaces\"";
                yield return " 12345";
                yield return " 12.345";
                yield return " -12345";
                yield return " -12.345";
            }
        }
    }
}
