using System;
using System.Threading.Tasks;

namespace Finite.Commands.AttributedModel
{
    internal static class CommandUtility
    {
        public static async ValueTask<ICommandResult> ExecuteAsync(
            ValueTask<ICommandResult> task)
        {
            return await task;
        }
    }
}
