using Finite.Commands.Binders;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal static class BinderUtility
    {
        private static readonly Type IParameterBinderType
            = typeof(IParameterBinder<>);

        private static readonly Type NumericBinderType
            = typeof(NumericBinder<>);

        public static void AddAllBinders(IServiceCollection services)
        {
            foreach (var (service, implementation) in GetAllBinderServices())
            {
                _ = services.AddSingleton(service, implementation);
            }
        }

        private static IEnumerable<(Type, Type)> GetAllBinderServices()
        {
            foreach (var binder in GetAllBinders())
                foreach (var impl in GetAllBinderImpls(binder))
                    yield return (impl, binder);

            static IEnumerable<Type> GetAllBinders()
            {
                yield return typeof(StringBinder);
                yield return typeof(GuidBinder);
                yield return typeof(BoolBinder);

                foreach (var type in NumericBinder.SupportedTypes.Keys)
                    yield return NumericBinderType.MakeGenericType(type);
            }

            static IEnumerable<Type> GetAllBinderImpls(Type binder)
            {
                foreach (var iface in binder.GetInterfaces())
                {
                    if (!iface.IsConstructedGenericType)
                        continue;

                    if (iface.GetGenericTypeDefinition()
                        != IParameterBinderType)
                        continue;

                    yield return iface;
                }
            }
        }
    }
}
