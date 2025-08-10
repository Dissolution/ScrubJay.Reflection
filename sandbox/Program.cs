using System.Diagnostics;
using ScrubJay.Reflection;
using ScrubJay.Reflection.Decompilation;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Text.Rendering;


var allMethods = Runtime.AllTypes.SelectMany(static type => type.GetMethods(Mirror.Flags.All));
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
        Debugger.Break();
    }
}


Console.WriteLine("Finished, press Enter to close");
Console.ReadLine();
return 0;