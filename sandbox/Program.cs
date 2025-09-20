using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ScrubJay.Extensions;
using ScrubJay.Reflection;
using ScrubJay.Reflection.Decompilation;
using ScrubJay.Reflection.Sandbox;
using ScrubJay.Reflection.Shards;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Text.Rendering;


var props =
    typeof(Util.NullabilityProperties)
        .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

foreach (var property in props)
{
    var r = RendererCache.Render(property);
    Console.WriteLine(r);
    Debugger.Break();

    // var attrs = Attribute.GetCustomAttributes(property);
    // var nullability = Nullability.Get(property);
    // var (pre, post) = nullability.GetPrefixPostfix();
    // if (attrs.TryGet<NullableAttribute>(out var nullableAttr))
    // {
    //     var flags = nullableAttr.NullableFlags;
    //     if (flags.Length != 1)
    //         Debugger.Break();
    //     var k = flags.ConvertAll(b => (NullabilityState)b)[0];
    //     Debugger.Break();
    // }

    // Debugger.Break();
}


/*
var allMethods = Runtime.AllTypes
    .SelectMany(static type => type.GetMethods(Mirror.Flags.ALL))
    .ToArray();

Random.Shared.Shuffle(allMethods);

foreach (var method in allMethods)
{
    try
    {
        var dm = new DecompiledMethod(method);
        var str = dm.Render();
        Console.WriteLine(str);
        Debugger.Break();
    }
    catch (Exception ex)
    {
        Console.Write(ex.Render());
        Console.WriteLine();
    }
}
*/

Console.WriteLine("Finished, press Enter to close");
Console.ReadLine();
return 0;