using System;
using NUnit.Framework;
using Finite.Commands;
using Finite.Commands.UnitTests;
using Finite.Commands.AttributedModel.SourceGenerator;
using Finite.Commands.AttributedModel;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Finite.Commands.Models.AttributedModel.SourceGenerator.UnitTests
{
    /// <summary>
    /// Unit tests for the attributed model source generator.
    /// </summary>
    public class AttributedModelSourceGeneratorTests
    {
        /// <summary>
        /// Ensures that the attributed model source generator only generates
        /// the data provider type when no commands are defined.
        /// </summary>
        [Test]
        public void NoAttributedModelCommandsGeneratesOnlyDataProvider()
        {
            var helper = new GeneratorTestHelper<AttributedModelSourceGenerator>(
                typeof(System.Reflection.Binder).Assembly,
                typeof(ICommandResult).Assembly,
                typeof(Module).Assembly
            );

            var result = helper.Run(
@"using System;

namespace UnitTestCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(""No commands"");
        }
    }
}");

            Assert.IsEmpty(result.Diagnostics);
            Assert.AreEqual(result.GeneratedTrees.Length, 1);

            var text = result.GeneratedTrees[0];
            var expected = CSharpSyntaxTree.ParseText(
@"using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands.AttributedModel.Internal.Commands
{
    internal static class DataProvider
    {
        private static readonly IAdditionalDataProviderFactory[] Factories
            = new IAdditionalDataProviderFactory[]
            {

            };

        public static IEnumerable<KeyValuePair<object, object?>> GetData(
            MethodInfo method)
        {
            foreach (var factory in Factories)
                foreach (var provider in factory.GetDataProvider(method))
                    foreach (var kvp in provider.GetData())
                        yield return kvp;
        }

        public static IEnumerable<KeyValuePair<object, object?>> GetData(
            ParameterInfo parameter)
        {
            foreach (var factory in Factories)
                foreach (var provider in factory.GetDataProvider(parameter))
                    foreach (var kvp in provider.GetData())
                        yield return kvp;
        }
    }
}");

            Assert.IsTrue(text.IsEquivalentTo(expected));
        }
    }
}
