using System.Reflection;
using System.Text;
using ScrubJay.Reflection.IL.Decompilation;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Utilities;

namespace ScrubJay.Reflection.Tests;

public static class TestData
{
    public readonly struct UnmanagedStruct
    {
        public readonly int Id;
        public readonly BindingFlags Flags;

        public UnmanagedStruct(int id, BindingFlags flags)
        {
            Id = id;
            Flags = flags;
        }

        public void Deconstruct(out int id, out BindingFlags flags)
        {
            id = Id;
            flags = Flags;
        }
    }
    
    public class ManagedClass : IDisposable
    {
        public Mirror? Mirror { get; set; }
        public ParameterInfo? Parameter { get; set; }

        public ManagedClass(Mirror? mirror, ParameterInfo? parameter)
        {
            Mirror = mirror;
            Parameter = parameter;
        }

        public void Dispose()
        {
            Mirror = null;
            Parameter = null;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types"/>
    public static readonly object[] UnmanagedObjects = 
    [
        short.MinValue,
        1313_147_147_147.000000000000014710m,
        (BindingFlags.Public | BindingFlags.Static),
        new ValueTuple<uint, char, bool>(13U, 'X', true),
        DateTime.Parse("1999-12-31 23:59:59"),
        Guid.Parse("00000000-DEAD-BEEF-CAFE-000000000000"),
        "Sphinx of black quartz, judge my vow",
        new UnmanagedStruct(147, BindingFlags.FlattenHierarchy),
        new Nullable<char>()!,
    ];

    public static readonly object[] ManagedObjects =
    [
        (new Tuple<int, string>(147, "TJ")),
        (new int[] { 1, 4, 7 }),
        (new List<char>("abc".ToCharArray())),
        (new ManagedClass(Mirror.Reflect<int>(), new DecompiledMethod.ThisParameterInfo(null!))),
        (new StringBuilder().Append("TRJ")),
    ];
    
    public static void AssertPropertiesEqual<T>(T? left, T? right)
    {
        Assert.Equal<bool>(left is null, right is null);
        if (left is null) return;

        var properties = Mirror.Reflect<T>()
            .Properties.Public.Instance
            .NotAnIndexer
            .ToList();

        if (properties.Count == 0)
        {
            Assert.Equal(left, right);
            return;
        }
        
        foreach (var property in properties)
        {
            var leftValue = property.GetValue(left);
            var rightValue = property.GetValue(right);
            AssertPropertiesEqual(leftValue, rightValue);
        }
    }
}

public static class AssertDeeper
{
    public static void AssertShallowEqual<T>(T? left, T? right)
    {
        if (EqualityComparer<T>.Default.Equals(left!, right!))
            return;
        
        var properties = Mirror
            .Reflect<T>()
            .Public.Instance.Properties
            .NotAnIndexer
            .ToList();
        
        foreach (var property in properties)
        {
            var leftValue = property.GetValue(left);
            var rightValue = property.GetValue(right);
            AssertShallowEqual(leftValue, rightValue);
        }
    }

    public static void AssertDeepEqual<T>(T? left, T? right)
    {
        Assert.Equal<bool>(left is null, right is null);
        if (left is null) return;

        Assert.Equal<T>(left, right);

        if (!typeof(T).IsPrimitive)
        {
            var fields = Mirror
                .Reflect<T>()
                .Instance.Fields
                .AsList();

            foreach (var field in fields)
            {
                // slow
                var leftValue = field.GetValue(left);
                var rightValue = field.GetValue(right);
                AssertDeepEqual(leftValue, rightValue);
            }
        }
    }
}