using System;
using System.Collections.Generic;
using System.Text;

namespace Wumpus.Commands
{
    public interface IResult
    {
        bool IsSuccess { get; }
    }
}
