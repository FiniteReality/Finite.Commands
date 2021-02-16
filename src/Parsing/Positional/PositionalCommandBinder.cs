using System;
using System.Diagnostics;
using static Finite.Commands.Parsing.ParameterHelper;

namespace Finite.Commands.Parsing
{
    internal sealed class PositionalCommandBinder : ICommandBinder
    {
        private readonly IParameterBinderFactory _binderFactory;

        public PositionalCommandBinder(IParameterBinderFactory binderFactory)
        {
            _binderFactory = binderFactory;
        }

        public bool TryBind(CommandContext context, ICommand command)
        {
            var tokenCount = (int)context.Items[TokenCount]!;

            for (int x = 0; x < command.Parameters.Count; x++)
            {
                var parameter = command.Parameters[x];
                var binder = _binderFactory.GetBinder(parameter.Type);
                CommandPath token;

                if (parameter is IRemainderParameter &&
                    x == command.Parameters.Count - 1 &&
                    x != tokenCount - 1)
                {
                    string rawToken = default!;
                    Index startIndex = default;
                    Index endIndex = default;

                    for (int pos = x; pos < tokenCount; pos++)
                    {
                        var originalToken = (CommandPath)context.Items[
                            GetParameterName(pos)]!;

                        if (pos == x)
                        {
                            rawToken = originalToken.RawValue;
                            startIndex = originalToken.Portion.Start;
                        }
                        else
                        {
                            Debug.Assert(rawToken == originalToken.RawValue);

                            endIndex = originalToken.Portion.End;
                        }
                    }

                    token = new CommandPath(rawToken,
                        startIndex..endIndex);
                }
                else
                {
                    token = (CommandPath)context.Items[GetParameterName(x)]!;
                }

                var value = binder.Bind(parameter, token.Value,
                    out var success);

                if (!success)
                    return false;

                context.Parameters[parameter.Name] = value;
            }

            return true;
        }
    }
}
