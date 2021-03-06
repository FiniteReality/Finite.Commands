using System.Collections.Generic;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines options for attributed commands
    /// </summary>
    public class AttributedCommandOptions
    {
        /// <summary>
        /// Gets or sets a list of assemblies to search for commands.
        /// </summary>
        public List<string> Assemblies { get; set; } = new List<string>();
    }
}
