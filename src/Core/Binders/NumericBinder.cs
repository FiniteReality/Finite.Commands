using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Finite.Commands.Binders
{
    /// <summary>
    /// Parameter binders for numeric types
    /// </summary>
    public sealed class NumericBinder
    {
        internal static Dictionary<Type, NumberStyles> SupportedTypes
            = new()
            {
                [typeof(sbyte)] = NumberStyles.Integer,
                [typeof(byte)] = NumberStyles.Integer,
                [typeof(short)] = NumberStyles.Integer,
                [typeof(ushort)] = NumberStyles.Integer,
                [typeof(int)] = NumberStyles.Integer,
                [typeof(uint)] = NumberStyles.Integer,
                [typeof(long)] = NumberStyles.Integer,
                [typeof(ulong)] = NumberStyles.Integer,
                [typeof(float)] = NumberStyles.Number,
                [typeof(double)] = NumberStyles.Number,
                [typeof(decimal)] = NumberStyles.Number,
            };

        /// <summary>
        /// Gets an object which can be used as a key in
        /// <see cref="IParameter.Data"/> to specify a specific
        /// <see cref="NumberStyles"/> to use instead of the default.
        /// </summary>
        public static object UseNumberStyle { get; } = new object();

        /// <summary>
        /// Gets an object which can be used as a key in
        /// <see cref="IParameter.Data"/> to specify a specific
        /// <see cref="NumberFormatInfo"/> to use instead of the default.
        /// </summary>
        public static object UseNumberFormat { get; } = new object();

        /// <summary>
        /// Creates a new <see cref="IParameterBinder{T}"/> for the given
        /// numeric type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="style">
        /// Overrides the default <see cref="NumberStyles"/> used during
        /// binding.
        /// </param>
        /// <param name="numberFormat">
        /// Overrides the default <see cref="NumberFormatInfo"/> used during
        /// binding.
        /// </param>
        /// <typeparam name="T">
        /// The numeric type to get a type binder for.
        /// </typeparam>
        /// <returns>
        /// A <see cref="IParameterBinder{T}"/> which can be used to bind
        /// parameters of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the passed type is not supported by the number binder.
        /// </exception>
        public static IParameterBinder<T> For<T>(
            NumberStyles? style = null,
            NumberFormatInfo? numberFormat = null)
            => SupportedTypes.ContainsKey(typeof(T))
                ? new NumericBinder<T>(style, numberFormat)
                : throw new NotSupportedException(
                    $"{typeof(T).FullName} is not supported");
    }


    internal sealed class NumericBinder<T> : IParameterBinder<T>
    {
        private delegate bool TryParseFunc(ReadOnlySpan<char> s,
            NumberStyles style, IFormatProvider? provider, out T result);

        private static readonly TryParseFunc TryParse;
        private static readonly NumberStyles DefaultStyle;

        private readonly NumberStyles _numberStyle;
        private readonly NumberFormatInfo _numberFormat;

        static NumericBinder()
        {
            Debug.Assert(NumericBinder.SupportedTypes.ContainsKey(typeof(T)));

            var method = typeof(T).GetMethod(nameof(TryParse),
                new[]
                {
                    typeof(ReadOnlySpan<char>),
                    typeof(NumberStyles),
                    typeof(IFormatProvider),
                    typeof(T).MakeByRefType()
                });

            Debug.Assert(method != null);

            TryParse = method.CreateDelegate<TryParseFunc>();
            DefaultStyle = NumericBinder.SupportedTypes[typeof(T)];
        }

        public NumericBinder()
            : this(null, null)
        { }

        public NumericBinder(NumberStyles? style, NumberFormatInfo? formatInfo)
        {
            _numberStyle = style ?? DefaultStyle;
            _numberFormat = formatInfo
                ?? CultureInfo.InvariantCulture.NumberFormat;
        }

        public T Bind(IParameter parameter, ReadOnlySpan<char> text,
            out bool success)
        {
            if (!parameter.TryGetData(NumericBinder.UseNumberStyle,
                out NumberStyles style))
                style = _numberStyle;

            if (!parameter.TryGetData(NumericBinder.UseNumberFormat,
                out NumberFormatInfo? formatInfo))
                formatInfo = _numberFormat;

            success = TryParse(text, style, formatInfo,
                out var value);

            return value;
        }
    }
}
