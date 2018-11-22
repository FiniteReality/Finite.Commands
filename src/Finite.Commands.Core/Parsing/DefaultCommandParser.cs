using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Default implementation of <see cref="ICommandParser{TContext}"/>
    /// which can be subclassed and overriden to provide enhanced features.
    /// </summary>
    public partial class DefaultCommandParser<TContext>
        : ICommandParser<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        // A list of default parsers for TryParseObject.
        private readonly Dictionary<Type, Func<string, (bool, object)>>
            _defaultParsers
            = new Dictionary<Type, Func<string, (bool, object)>>()
            {
                [typeof(sbyte)] = (x) => (sbyte.TryParse(x, out var y), y),
                [typeof(byte)] = (x) => (byte.TryParse(x, out var y), y),

                [typeof(short)] = (x) => (short.TryParse(x, out var y), y),
                [typeof(ushort)] = (x) => (ushort.TryParse(x, out var y), y),

                [typeof(int)] = (x) => (int.TryParse(x, out var y), y),
                [typeof(uint)] = (x) => (uint.TryParse(x, out var y), y),

                [typeof(long)] = (x) => (long.TryParse(x, out var y), y),
                [typeof(ulong)] = (x) => (ulong.TryParse(x, out var y), y),
                [typeof(string)] = (x) => (true, x)
            };

        /// <summary>
        /// Attempts to deserialize a parameter into a given type
        /// </summary>
        /// <param name="param">
        /// The parameter to deserialize <paramref name="value"/> for.
        /// </param>
        /// <param name="value">
        /// A string containing the value of the parameter to deserialize.
        /// </param>
        /// <param name="result">
        /// The parsed result, boxed in an object.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the parse was successful or not
        /// </returns>
        protected virtual bool TryParseObject(ICommandService commands,
            ParameterInfo param, string value, out object result)
        {
            var factory = commands.TypeReaderFactory;
            if (factory.TryGetTypeReader(param.Type, out var reader))
            {
                return reader.TryRead(value, out result);
            }
            else if (_defaultParsers.TryGetValue(param.Type, out var parser))
            {
                var (success, parsed) = parser(value);
                result = parsed;
                return success;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to deserialize the arguments for a given comand match.
        /// </summary>
        /// <param name="match">
        /// The <see cref="CommandMatch"/> to deserialize arguments for.
        /// </param>
        /// <param name="result">
        /// The parsed arguments for this match.
        /// </param>
        /// <returns>
        /// A tuple containing a <see cref="bool"/> representing success, and
        /// an array of parameters which will be <code>null</code> if the
        /// former value is <code>false</code>.
        /// </returns>
        protected virtual bool GetArgumentsForMatch(ICommandService commands,
            CommandMatch match, out object[] result)
        {
            bool TryParseMultiple(ParameterInfo argument, int startPos,
                out object[] parsed)
            {
                parsed = new object[match.Arguments.Length - startPos];
                for (int i = startPos; i < match.Arguments.Length; i++)
                {
                    var ok = TryParseObject(commands, argument, match.Arguments[i],
                        out var value);

                    if (!ok)
                        return false;

                    parsed[i - startPos] = value;
                }

                return true;
            }

            var parameters = match.Command.Parameters;
            result = new object[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                var argument = parameters[i];
                if ((i == parameters.Count - 1) &&
                    argument.Attributes.Any(x => x is ParamArrayAttribute))
                {
                    if (!TryParseMultiple(argument, i, out var multiple))
                        return false;
    
                    result[i] = multiple;
                }
                else
                {
                    var ok = TryParseObject(commands, argument,
                        match.Arguments[i], out var value);

                    if (!ok)
                        return false;

                    result[i] = value;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public virtual IResult Parse(CommandExecutionContext executionContext)
        {
            var result = Tokenize(executionContext.Context.Message,
                executionContext.PrefixLength);

            if (!result.IsSuccess)
                return result;

            string[] tokenStream = result.TokenStream;
            var commands = executionContext.CommandService;

            foreach (var match in commands.FindCommands(tokenStream))
            {
                if (GetArgumentsForMatch(executionContext.CommandService,
                    match, out object[] arguments))
                {
                    // TODO: maybe I should migrate this to a parser result?
                    executionContext.Command = match.Command;
                    executionContext.Arguments = arguments;

                    return SuccessResult.Instance;
                }
            }

            return CommandNotFoundResult.Instance;
        }
    }
}
