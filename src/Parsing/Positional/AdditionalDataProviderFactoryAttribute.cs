using System;

[assembly: Finite.Commands.AttributedModel.AdditionalDataProviderFactory("Finite.Commands.Parsing.Internal.PositionalDataProviderFactory")]

namespace Finite.Commands.AttributedModel
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal class AdditionalDataProviderFactoryAttribute : Attribute
    {
        public string FactoryType { get; }

        public AdditionalDataProviderFactoryAttribute(string providerType)
        {
            FactoryType = providerType;
        }
    }
}
