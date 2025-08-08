

using System.Reflection.Emit;
using ScrubJay.Reflection.Utilities;

var longest = OpCodeHelper.OpCodes
    .Select(op => op.Name)
    .Select(name => name.Length)
    .Max();



Console.WriteLine("Finished, press Enter to close");
Console.ReadLine();
return 0;