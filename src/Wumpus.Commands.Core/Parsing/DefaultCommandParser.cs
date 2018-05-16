using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public partial class DefaultCommandParser : ICommandParser
    {
        private readonly Dictionary<Type, Func<string, Task<object>>> _defaultParsers
            = new Dictionary<Type, Func<string, Task<object>>>()
            {
                [typeof(sbyte)] = (x) => Task.FromResult<object>(sbyte.Parse(x)),
                [typeof(byte)] = (x) => Task.FromResult<object>(byte.Parse(x)),

                [typeof(short)] = (x) => Task.FromResult<object>(short.Parse(x)),
                [typeof(ushort)] = (x) => Task.FromResult<object>(ushort.Parse(x)),

                [typeof(int)] = (x) => Task.FromResult<object>(int.Parse(x)),
                [typeof(uint)] = (x) => Task.FromResult<object>(uint.Parse(x)),

                [typeof(long)] = (x) => Task.FromResult<object>(long.Parse(x)),
                [typeof(ulong)] = (x) => Task.FromResult<object>(ulong.Parse(x)),
            };

        public virtual async Task<(bool, object)> TryParseObjectAsync(
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

        public virtual async Task<(bool, object[])> GetArgumentsForMatchAsync(
            CommandMatch match)
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

        public virtual async Task ParseAsync<TContext>(
            CommandExecutionContext executionContext)
            where TContext : class, ICommandContext<TContext>
        {
            string[] tokenStream = Tokenize(executionContext.Context.Message);
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
