using System;

namespace Finite.Commands
{
    internal sealed class ParameterBinderWrapper<T>
        : IParameterBinder<object>
    {
        private readonly IParameterBinder<T> _inner;

        public ParameterBinderWrapper(IParameterBinder<T> inner)
        {
            _inner = inner;
        }

        public object? Bind(ReadOnlySpan<char> text, out bool success)
            => _inner.Bind(text, out success);
    }
}
