using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Text.Rendering;
using ScrubJay.Utilities;


namespace ScrubJay.Reflection.Tests;

public static class AssertDeeper
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        Converters =
        {

        },
        IncludeFields = true,
    };

    private static void AssertDefaultEqual<T>(T? left, T? right)
    {
        try
        {
            Assert.Equal<T>(left, right);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Render());
            Debugger.Break();
        }
       
    }

    private static void AssertJsonEqual<T>(T? left, T? right)
    {
        var leftJson = JsonSerializer.Serialize(left, _jsonSerializerOptions);
        var rightJson = JsonSerializer.Serialize(right, _jsonSerializerOptions);
        Assert.Equal(leftJson, rightJson);
    }

    private static void AssertDeepFieldsEqual<T>(T? left, T? right)
    {
        Assert.Equal<bool>(left is null, right is null);
        if (left is null) return;

        if (typeof(T).IsPrimitive)
        {
            AssertDefaultEqual(left, right);
        }
        else
        {
            var fields = Mirror
                .Shard<T>()
                .Instance()
                .Fields()
                .ToList();

            foreach (var field in fields)
            {
                // slow
                var leftValue = field.GetValue(left);
                var rightValue = field.GetValue(right);
                AssertDeepFieldsEqual(leftValue, rightValue);
            }
        }
    }

    private static void AssertUnmanagedEqual<U>(U left, U right)
    {
        Assert.True(typeof(U).IsUnmanaged());
        
        Span<byte> leftBytes = new byte[Notsafe.SizeOf<U>()];
        Unsafe.WriteUnaligned(ref leftBytes.GetPinnableReference(), left);
            
        Span<byte> rightBytes = new byte[Notsafe.SizeOf<U>()];
        Unsafe.WriteUnaligned(ref rightBytes.GetPinnableReference(), right);
            
        Assert.Equal<byte>(leftBytes, rightBytes);
    }

    public static void IsDeepClone<T>(T? original, T? clone)
    {
        AssertDefaultEqual<T>(original, clone);
        if (typeof(T).IsUnmanaged())
        {
            Assert.NotNull(original);
            Assert.NotNull(clone);
            AssertUnmanagedEqual<T>(original, clone);
        }
        AssertJsonEqual<T>(original, clone);
        AssertDeepFieldsEqual<T>(original, clone);
    }
}