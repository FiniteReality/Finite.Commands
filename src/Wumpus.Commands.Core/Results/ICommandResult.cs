using System;
using System.Collections.Generic;
using System.Text;

namespace Wumpus.Commands
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }
    }
}
