global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using text = System.ReadOnlySpan<char>;

using System.Diagnostics;
using System.Dynamic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ScrubJay.Debugging;
using ScrubJay.Enums;
using ScrubJay.Functional;
using ScrubJay.Reflection.IL;
using ScrubJay.Reflection.IL.Decompilation;
using ScrubJay.Reflection.IL.Instructions;
using ScrubJay.Reflection.Sandbox;
using ScrubJay.Reflection.Utilities;

Console.OutputEncoding = System.Text.Encoding.UTF8;


var console = DynamicWrapper.WrapStaticType(typeof(Console));

console.Write('a').Write(147).Write(DateTime.Now).WriteLine();

Debugger.Break();





var methods = TypeHelper
    .GetAllTypes()
    .SelectMany(static type => Reflect(type).Methods.AsList())
    .Distinct()
    // Abstract methods cannot have a body
    .Where(static method => !method.IsAbstract)
    // DllImports do not declare a body
    .Where(static method => !method.HasAttribute<DllImportAttribute>())
    // Internal calls do not have a body, nor do runtime provided calls
    .Where(static method => !method.MethodImplementationFlags.HasAnyFlags(
        MethodImplAttributes.InternalCall,
        MethodImplAttributes.Runtime))
    .ToList();

int count = methods.Count;
for (var i = 0; i < count; i++)
{
    var method = methods[i];

    var tryDecompile = DecompiledMethod.TryDecompile(method);
    if (tryDecompile.IsOkWithError(out var ok, out var error))
    {
        var display = ok.ToString();
        if (ok.Instructions.Count >= 20)
        {
            Console.Clear();
            Console.WriteLine(display);
            Debugger.Break();
        }
        //Console.Clear();
        //Console.WriteLine(display);
    }
    else
    {
        Debug.WriteLine(error.Dump());
        Debugger.Break();
    }
}


Debugger.Break();

//
//var opCodes = OpCoding.AllOpCodes;
//Debugger.Break();
//
//
//DynamicMethod method = RuntimeBuilder.CreateDynamicMethod<Action>();
//var gen = method.GetILGenerator();
//gen.Emit(OpCodes.Ret);
//var act = method.CreateDelegate<Action>();
//act();
//
//byte[] il = ReflectionExtensions.GetILBytes(method);
//
//Expression<Func<int>> expr = () => 147;
//var il2 = ReflectionExtensions.GetILBytes(expr.Compile().Method);

Debugger.Break();
return 0;

