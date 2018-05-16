using System.Collections.Generic;
using System.Linq;

namespace Wumpus.Commands
{
    internal class CommandMap
    {
        private readonly CommandMapNode _root;

        public CommandMap(IReadOnlyList<ModuleInfo> modules = null)
        {
            _root = new CommandMapNode();

            void AddCommands(ModuleInfo module, Stack<string> path)
            {
                foreach (var moduleAlias in module.Aliases)
                {
                    path.Push(moduleAlias);

                    foreach (var command in module.Commands)
                    {
                        foreach (var alias in command.Aliases)
                        {
                            path.Push(alias);
                            AddCommand(path.ToArray(), command);
                            path.Pop();
                        }
                    }

                    foreach (var submodule in module.Submodules)
                        AddCommands(submodule, path);

                    path.Pop();
                }
            }

            if (modules != null)
            {
                Stack<string> pathStack = new Stack<string>();
                foreach (var module in modules)
                    AddCommands(module, pathStack);
            }
        }

        public IEnumerable<CommandMatch> GetCommands(string[] commandPath)
            => _root.FindCommands(commandPath, 0);

        public bool AddCommand(string[] path, CommandInfo command)
            => _root.Add(command, path, 0);

        public bool RemoveCommand(string[] path, out CommandInfo command)
            => _root.Remove(path, out command, 0);

        private class CommandMapNode
        {
            private readonly Dictionary<string, CommandInfo> _commands;
            private readonly Dictionary<string, CommandMapNode> _nodes;

            public CommandMapNode()
            {
                _commands = new Dictionary<string, CommandInfo>();
                _nodes = new Dictionary<string, CommandMapNode>();
            }

            public IEnumerable<CommandMatch> FindCommands(string[] segments,
                int startIndex)
            {
                var segment = segments[startIndex];

                if (startIndex < segment.Length - 1 &&
                    _nodes.TryGetValue(segment, out var node))
                {
                    var commands = node.FindCommands(segments,
                        startIndex + 1);
                    foreach (var command in commands)
                        yield return command;
                }

                if (_commands.TryGetValue(segment, out var rootCommand))
                {
                    yield return new CommandMatch
                    {
                        Command = rootCommand,
                        Arguments = segments.Skip(startIndex + 1).ToArray()
                    };
                }
            }

            public bool Add(CommandInfo command, string[] segments,
                int startIndex)
            {
                if (startIndex == segments.Length - 1)
                {
                    return _commands.TryAdd(segments[startIndex], command);
                }
                else
                {
                    var segment = segments[startIndex];
                    var node = _nodes.GetOrAdd(segment,
                        (_) => new CommandMapNode());

                    return node.Add(command, segments, startIndex + 1);
                }
            }

            public bool Remove(string[] segments, out CommandInfo command,
                int startIndex)
            {
                if (startIndex == segments.Length - 1)
                {
                    return _commands.TryRemove(segments[startIndex],
                        out command);
                }
                else
                {
                    var segment = segments[startIndex];
                    if (_nodes.TryGetValue(segment, out var node))
                        return node.Remove(segments, out command,
                            startIndex + 1);
                }

                command = null;
                return false;
            }
        }
    }
}
