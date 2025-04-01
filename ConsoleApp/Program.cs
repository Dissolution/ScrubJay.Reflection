using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using ScrubJay.Reflection.Decompiling;
using ScrubJay.Reflection.Runtime;
using Exception = System.Exception;

var prop = typeof(Point).GetProperty("X", BindingFlags.Public | BindingFlags.Instance);

var getMethod = prop.GetMethod;


Func<Point, int> del = RuntimeBuilder.EmitDelegate<Func<Point, int>>(emitter =>
{
    emitter
        .Ldarga(0)
        .Callvirt(getMethod)
        .Ret();
});

Point pt = new Point(3, 4);
var x = del(pt);

/*

ILGeneratorPoking.Poke();*/

Debugger.Break();
Console.WriteLine("Press Enter to close");
Console.ReadLine();
return;

namespace ScrubJay.Reflection.ConsoleApp
{
    static class ILGeneratorPoking
    {
        public static void Poke()
        {
            var dynamicMethod = RuntimeBuilder.CreateDynamicMethod<Action>("act");
            var ilGenerator = dynamicMethod.GetILGenerator();

            var opCodes = DecompiledMethod.AllOpCodes;
            int maxNameLength = int.MinValue;
            foreach (var opCode in opCodes)
            {
                int offset = ilGenerator.ILOffset;
                if (opCode.Name is not null)
                {
                    if (opCode.Name.Length > maxNameLength)
                        maxNameLength = opCode.Name.Length;
                    if (opCode.Name.Length > 9)
                        Debugger.Break();
                }

                try
                {
                    ilGenerator.Emit(opCode);
                    int newOffset = ilGenerator.ILOffset;
                    int size = newOffset - offset;
                    if (size != opCode.Size)
                    {
                        Debugger.Break();
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
            }

            Debugger.Break();
        }
    }
}
