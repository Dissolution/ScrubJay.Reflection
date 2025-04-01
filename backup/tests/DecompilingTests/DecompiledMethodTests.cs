using System.Diagnostics;
using System.Reflection;
using Polyfills;
using ScrubJay.Extensions;
using ScrubJay.Reflection.Decompiling;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Tests.DecompilingTests;

public class DecompiledMethodTests
{
    [Fact]
    public void DecompileWorks()
    {
        var assemblyTypes = AssemblyHelper.AllAssemblyTypes;
        var staticMethods = assemblyTypes
            .Where(type => type.IsStatic())
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(method => method.IsStatic)
            //.Where(method => !method.Attributes.HasFlag(MethodAttributes.HideBySig))
            .ToArray();

        new Random().Shuffle(staticMethods);
        var firstHundred = staticMethods.AsSpan(0, 100);
        foreach (var method in firstHundred)
        {
            var result = DecompiledMethod.TryDecompile(method);
            Assert.True(result.IsOk(out var decompiledMethod));
            var instructions = decompiledMethod.OpCodeInstructions;
            Assert.NotNull(instructions);
            Assert.True(instructions.Count > 0);
            if (instructions.Count >= 100)
            {
                var il = decompiledMethod.ToString();
                Debugger.Break();
            }
        }
    }
}
