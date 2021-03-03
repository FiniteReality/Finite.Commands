using System.Reflection.Emit;

namespace Finite.Commands.AttributedModel
{
    internal static class ILGeneratorExtensions
    {
        public static void EmitLoadConstant(this ILGenerator generator,
            int constant)
        {
            {
                var opcode = constant switch
                {
                    0 => OpCodes.Ldc_I4_0,
                    1 => OpCodes.Ldc_I4_1,
                    2 => OpCodes.Ldc_I4_2,
                    3 => OpCodes.Ldc_I4_3,
                    4 => OpCodes.Ldc_I4_4,
                    5 => OpCodes.Ldc_I4_5,
                    6 => OpCodes.Ldc_I4_6,
                    7 => OpCodes.Ldc_I4_7,
                    8 => OpCodes.Ldc_I4_8,
                    < 255 => OpCodes.Ldc_I4_S,
                    _ => OpCodes.Ldc_I4
                };

                if (opcode == OpCodes.Ldc_I4_S)
                    generator.Emit(opcode, (byte)constant);
                else if (opcode == OpCodes.Ldc_I4)
                    generator.Emit(opcode, constant);
                else
                    generator.Emit(opcode);
            }
        }
    }
}
