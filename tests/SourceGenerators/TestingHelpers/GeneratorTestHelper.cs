using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Finite.Commands.UnitTests
{
    /// <summary>
    /// Defines a class which can be used to run source generator tests.
    /// </summary>
    public class GeneratorTestHelper<TGenerator>
        where TGenerator : ISourceGenerator, new()
    {
        private readonly GeneratorDriver _driver;
        private readonly TGenerator _generator;
        private readonly Assembly[] _references;

        /// <summary>
        /// Gets the source generator instance used.
        /// </summary>
        public TGenerator Generator => _generator;

        /// <summary>
        /// Creates a new instance of the <see cref="GeneratorTestHelper{T}"/>
        /// class.
        /// </summary>
        public GeneratorTestHelper(params Assembly[] references)
        {
            _generator = new TGenerator();
            _driver = CSharpGeneratorDriver.Create(_generator);
            _references = references;
        }

        /// <summary>
        /// Runs the generator with the given source code.
        /// </summary>
        /// <param name="sourceCode">
        /// The source code to pass to the generator.
        /// </param>
        /// <returns>
        /// Returns a <see cref="GeneratorDriverRunResult"/> containing the
        /// result of running the generator with the given source code.
        /// </returns>
        public GeneratorDriverRunResult Run(string sourceCode)
        {
            var compilation = CSharpCompilation.Create("UnitTest",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) },
                references: _references
                    .Select(x => MetadataReference.CreateFromFile(x.Location)),
                options: new CSharpCompilationOptions(
                        OutputKind.DynamicallyLinkedLibrary));

            var resultDriver = _driver.RunGenerators(compilation);
            return resultDriver.GetRunResult();
        }
    }
}
