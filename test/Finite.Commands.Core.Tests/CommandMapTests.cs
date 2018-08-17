using System;
using System.Linq;
using Xunit;

namespace Finite.Commands.Tests
{
    public class CommandMapTests
    {
        [Theory]
        [InlineData("nonNested")]
        [InlineData("nested", "command")]
        [InlineData("a", "third", "level")]
        void AddRemove(params string[] path)
        {
            var map = new CommandMap();
            var testCommand = new CommandInfo(null, null, null, null,
                Array.Empty<ParameterBuilder>());
            var testCommand2 = new CommandInfo(null, null, null, null,
                Array.Empty<ParameterBuilder>());

            Assert.True(map.AddCommand(path, testCommand));
            Assert.False(map.AddCommand(path, testCommand2));
            Assert.True(map.RemoveCommand(path,
                out var removedCommand));
            Assert.Equal(testCommand, removedCommand);

            Assert.True(map.AddCommand(path, testCommand2));
            Assert.False(map.AddCommand(path, testCommand));
            Assert.True(map.RemoveCommand(path,
                out removedCommand));
            Assert.Equal(testCommand2, removedCommand);
        }

        [Theory]
        [InlineData(
            new string[]{"nested", "command"},
            new string[]{"nested"},
            new string[]{"nested", "command"},
            new string[]{"not", "a", "command"})]
        void FindCommands(string[] command1Path, string[] command2Path,
            string[] searchPath, string[] invalidSearchPath)
        {
            var map = new CommandMap();
            var testCommand = new CommandInfo(null, null, null, null,
                Array.Empty<ParameterBuilder>());
            var testCommand2 = new CommandInfo(null, null, null, null,
                Array.Empty<ParameterBuilder>());

            Assert.True(map.AddCommand(command1Path, testCommand));
            Assert.True(map.AddCommand(command2Path, testCommand2));

            var commands = map.GetCommands(searchPath);
            Assert.NotNull(commands);
            var commandsArray = commands.ToArray();
            Assert.Equal(commandsArray.Length, 2);

            commands = map.GetCommands(invalidSearchPath);
            Assert.NotNull(commands);
            commandsArray = commands.ToArray();
            Assert.Empty(commandsArray);
        }

        [Theory]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module",
            1)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module ThisIsAnArgument",
            1)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module stat",
            2)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module stat ThisIsAnArgument",
            2)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module stats",
            2)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "module stats ThisIsAnArgument",
            2)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "unrelated",
            0)]
        [InlineData(
            new string[]{"module", "module stat", "module stats"},
            "",
            0)]
        void FindCommandsDefaultAlias(string[] aliases,
            string searchQuery, int expectedQueryResults)
        {
            var map = new CommandMap();
            var testCommand = new CommandInfo(null, null, null, null,
                Array.Empty<ParameterBuilder>());

            foreach (var alias in aliases)
            {
                string[] path = alias.Split(' ');

                Assert.True(map.AddCommand(path, testCommand));
            }

            var commands = map.GetCommands(searchQuery.Split(' '));
            Assert.NotNull(commands);

            var commandsArray = commands.ToArray();
            Assert.Equal(expectedQueryResults, commandsArray.Length);
        }
    }
}
