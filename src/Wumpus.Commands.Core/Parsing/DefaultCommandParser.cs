using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// Default implementation of <see cref="ICommandParser"/> which can be
    /// subclassed and overriden to provide enhanced features.
    /// </summary>
    public partial class DefaultCommandParser : ICommandParser
    {
        // A list of default parsers for TryParseObjectAsync.
        // TODO: migrate this to a typereader-style API
        private readonly Dictionary<Type, Func<string, Task<object>>>
            _defaultParsers
            = new Dictionary<Type, Func<string, Task<object>>>()
            {
                [typeof(sbyte)] = (x) => Task.FromResult<object>(
                    sbyte.Parse(x)),
                [typeof(byte)] = (x) => Task.FromResult<object>(byte.Parse(x)),

                [typeof(short)] = (x) => Task.FromResult<object>(
                    short.Parse(x)),
                [typeof(ushort)] = (x) => Task.FromResult<object>(
                    ushort.Parse(x)),

                [typeof(int)] = (x) => Task.FromResult<object>(int.Parse(x)),
                [typeof(uint)] = (x) => Task.FromResult<object>(uint.Parse(x)),

                [typeof(long)] = (x) => Task.FromResult<object>(long.Parse(x)),
                [typeof(ulong)] = (x) => Task.FromResult<object>(
                    ulong.Parse(x)),
            };

        /// <summary>
        /// Attempts to deserialize a parameter into a given type
        /// </summary>
        /// <param name="type">
        /// The type to deserialize <paramref name="value"/> into.
        /// </param>
        /// <param name="value">
        /// A string containing the value of the parameter to deserialize.
        /// </param>
        /// <returns>
        /// A tuple containing a <see cref="bool"/> representing success, and a
        /// <see cref="object"/> which will be null if the
        /// former value is <code>false</code>.
        /// </returns>
        protected virtual async Task<(bool, object)> TryParseObjectAsync(
            Type type, string value)
        {
            if (type == typeof(string))
                return (true, value);

            if (_defaultParsers.TryGetValue(type, out var parser))
            {
                var result = await parser(value)
                    .ConfigureAwait(false);
                return (true, result);
            }

            return (false, null);
        }

        /// <summary>
        /// Attempts to deserialize the arguments for a given comand match.
        /// </summary>
        /// <param name="match">
        /// The <see cref="CommandMatch"/> to deserialize arguments for.
        /// </param>
        /// <returns>
        /// A tuple containing a <see cref="bool"/> representing success, and
        /// an array of parameters which will be <code>null</code> if the
        /// former value is <code>false</code>.
        /// </returns>
        protected virtual async Task<(bool, object[])>
            GetArgumentsForMatchAsync(CommandMatch match)
        {
            var arguments = match.Arguments;

            // Drop extra params silently and assume our parameters are in order
            // TODO: support remainder params
            if (match.Arguments.Length > match.Command.Parameters.Count)
                Array.Resize(ref arguments,
                    match.Command.Parameters.Count);

            object[] result = new object[arguments.Length];

            int i = 0;
            foreach (var argument in arguments)
            {
                var argumentInfo = match.Command.Parameters.ElementAt(i);
                var (success, parsed) = await TryParseObjectAsync(
                    argumentInfo.Type, argument)
                    .ConfigureAwait(false);

                if (!success)
                    return (false, null);

                result[i] = parsed;
                i++;
            }

            return (true, result);
        }

        /// <inheritdoc/>
        public virtual async Task ParseAsync<TContext>(
            CommandExecutionContext executionContext)
            where TContext : class, ICommandContext<TContext>
        {
            string[] tokenStream = Tokenize(executionContext.Context.Message,
                executionContext.PrefixLength);
            var commands = executionContext.CommandService;

            foreach (var match in commands.FindCommands(tokenStream))
            {
                var (success, arguments) = await GetArgumentsForMatchAsync(
                    match)
                    .ConfigureAwait(false);

                if (success)
                {
                    executionContext.Command = match.Command;
                    executionContext.Arguments = arguments;

                    break;
                }
            }
        }
    }
}
