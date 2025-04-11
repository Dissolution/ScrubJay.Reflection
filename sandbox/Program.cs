global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using text = System.ReadOnlySpan<char>;

using System.Diagnostics;
using System.Reflection.Emit;
using ScrubJay.Debugging;
using ScrubJay.Functional;
using ScrubJay.Reflection.IL;
using ScrubJay.Reflection.IL.Decompilation;

Console.OutputEncoding = System.Text.Encoding.UTF8;


var groupedCodes =
OpCoding.AllOpCodes
    .GroupBy(static op => op.OperandType)
    .Select(static grouping => (grouping.Key, grouping.ToArray()))
    .OrderBy(static tuple => tuple.Item2.Length)
    .Select(tuple => (tuple.Item1, string.Join(", ", tuple.Item2)))
    .ToList();



/*

foreach (var op in OpCoding.AllOpCodes)
{
    var isLoc = op.TargetsArgument(out var index);
    if (isLoc && index.IsSome())
        Debugger.Break();
}
    


var locNames =
    OpCoding.AllOpCodes
        .Where(op => TextHelper.Contains(op.Name, "loc", StringComparison.OrdinalIgnoreCase))
        .Select(op => op.Name)
        .WhereNotNull()
        .ToList();



var names = TextBuilder.New.DelimitAppend(Environment.NewLine, locNames).ToStringAndDispose();
Debugger.Break();
    
*/


var methods = Util.GetAllTypes()
    .SelectMany(static type => Reflect(type).Methods.AsList())
    .ToArray();
Random.Shared.Shuffle(methods);

foreach (var method in methods)
{
    try
    {
        var decom = DecompiledMethod.Decompile(method);
        var instructions = decom.ReadInstructions();
        var il = instructions.ToString();
        Console.Clear();
        Console.WriteLine(il);
        Debugger.Break();
    }
    catch (Exception ex)
    {
        Debug.WriteLine(ex.Dump());
    }
   
}

//
//var opcodes = OpCoding.AllOpCodes;
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



internal static class Util
{
    public static HashSet<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(static ass => Result.TryInvoke(ass.GetTypes).OkOr([]))
            .ToHashSet();
    }

    public static bool IsLoc(OpCode opcode)
    {
        return MemoryExtensions.Contains(opcode.Name.AsSpan(), "loc".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }
}
