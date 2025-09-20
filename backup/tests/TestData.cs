using System.Reflection;
using System.Text;
using ScrubJay.Reflection.Searching;

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
        public Nullable<int> Id { get; set; } = null;
        public string? Name { get; set; } = null;

        public ManagedClass(int? id, string? name)
        {
            Id = id;
            Name = name;
        }

        public void Dispose()
        {
            Id = null;
            Name = null;
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
        (DateTime)DateTime.Parse("1999-12-31 23:59:59"),
        (Guid)Guid.Parse("00000000-DEAD-BEEF-CAFE-000000000000"),
        (UnmanagedStruct)new UnmanagedStruct(147, BindingFlags.FlattenHierarchy),
    ];

    public static readonly object[] ManagedObjects =
    [
        //new Nullable<char>('j'),
        //new Nullable<char>(),
        (new Tuple<int, string>(147, "TJ")),
        (new int[] { 1, 4, 7 }),
        (new List<char>("abc".ToCharArray())),
        (new ManagedClass(147, "TJ")),
        (new StringBuilder().Append("TRJ")),
    ];

    public static IEnumerable<Guid> InfiniteGuids()
    {
        while (true)
            yield return Guid.NewGuid();
    }
    
    
    public static void AssertPropertiesEqual<T>(T? left, T? right)
    {
        Assert.Equal<bool>(left is null, right is null);
        if (left is null) return;

        var properties = Mirror.Shard<T>()
            .Properties().Public().Instance()
            .NonIndexers()
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