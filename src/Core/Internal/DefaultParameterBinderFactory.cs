using System;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal sealed class DefaultParameterBinderFactory
        : IParameterBinderFactory
    {
        private static readonly Type ParameterBinderType
            = typeof(IParameterBinder<>);

        private static readonly Type ParameterBinderWrapperType
            = typeof(ParameterBinderWrapper<>);

        private readonly IServiceProvider _serviceProvider;

        public DefaultParameterBinderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IParameterBinder<object> GetBinder(Type type)
        {
            return (IParameterBinder<object>)_serviceProvider
                .GetRequiredService(
                    (type.IsValueType
                        ? ParameterBinderWrapperType
                        : ParameterBinderType)
                    .MakeGenericType(type));
        }
    }
}
