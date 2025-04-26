global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using text = System.ReadOnlySpan<char>;

using System.Diagnostics;
using System.Runtime.InteropServices;
using ScrubJay.Debugging;
using ScrubJay.Enums;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.IL.Decompilation;
using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Utilities;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var emitter = new CleanEmitter(default!);


/* DECOMPILE EVERYTHING!
var methods = TypeHelper
    .GetAllTypes()
    .SelectMany(static type => Reflect(type).Methods().AsList())
    .Distinct()
    // Abstract methods cannot have a body
    .Where(static method => !method.IsAbstract)
    // DllImports do not declare a body
    .Where(static method => !method.HasAttribute<DllImportAttribute>())
    // Internal calls do not have a body, nor do runtime provided calls
    .Where(static method => !method.MethodImplementationFlags.HasAnyFlags(
        MethodImplAttributes.InternalCall,
        MethodImplAttributes.Runtime))
    .ToArray();
Random.Shared.Shuffle(methods);

int count = methods.Length;
for (var i = 0; i < count; i++)
{
    var method = methods[i];

    var tryDecompile = DecompiledILMethod.TryDecompile(method);
    if (tryDecompile.IsOkWithError(out var ok, out var error))
    {
        var display = ok.ToString();
        if (ok.Instructions.Count >= 200)
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
*/




Debugger.Break();
return 0;



