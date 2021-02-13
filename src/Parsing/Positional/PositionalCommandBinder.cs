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
            for (int x = 0; x < command.Parameters.Count; x++)
            {
                var parameter = command.Parameters[x];
                var binder = _binderFactory.GetBinder(parameter.Type);

                var token = (CommandPath)context.Items[GetParameterName(x)]!;

                var value = binder.Bind(token.Value, out var success);

                if (!success)
                    return false;

                context.Parameters[parameter.Name] = value;
            }

            return true;
        }
    }
}
