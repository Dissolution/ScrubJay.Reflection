using System.Reflection;
using ScrubJay.Reflection.Cloning;

namespace ScrubJay.Reflection.Tests.CloningTests;

public class DeepCloneTests
{
    public static TheoryData<object> UnmanagedObjects { get; } =
    [
        short.MinValue,
        1313_147_147_147.000000000000014710m,
        BindingFlags.Public | BindingFlags.Static,
        new ValueTuple<uint, char, bool>(13U, 'X', true),
        DateTime.Parse("1999-12-31 23:59:59"),
        Guid.Parse("00000000-DEAD-BEEF-CAFE-000000000000"),
        new TestData.UnmanagedStruct(147, BindingFlags.FlattenHierarchy),
    ];
    

    public static TheoryData<object> ManagedObjects { get; } = new(TestData.ManagedObjects);

    [Fact]
    public void CanDeepCloneNull()
    {
        var clone = Cloner.DeepClone<object>(null);
        Assert.Null(clone);

        clone = Cloner.DeepClone<string>(null);
        Assert.Null(clone);

        clone = Cloner.DeepClone<int?>(null);
        Assert.Null(clone);
    }

    public static TheoryData<string> TestStrings { get; } = new() { "", "147", Guid.NewGuid().ToString(), "Sphinx of black quartz, judge my vow", };
    
    [Theory]
    [MemberData(nameof(TestStrings))]
    public void CanDeepCloneString(string str)
    {
        string clone = Cloner.DeepClone<string>(str);
        AssertDeeper.IsDeepClone(str, clone);
    }
    
    [Theory]
    [MemberData(nameof(UnmanagedObjects))]
    public void CanDeepCloneUnmanaged<T>(T @unmanaged)
        where T : unmanaged
    {
        var clone = Cloner.DeepClone<T>(unmanaged);
        AssertDeeper.IsDeepClone(@unmanaged, clone);
    }

    [Theory]
    [MemberData(nameof(ManagedObjects))]
    public void CanDeepCloneManaged<T>(T managed)
    {
        var clone = Cloner.DeepClone<T>(managed);
        if (managed is null)
        {
            Assert.Null(clone);
        }
        else
        {
            Assert.False(ReferenceEquals(managed, clone));
        }
        
        AssertDeeper.IsDeepClone(managed, clone);
    }
    
    
    
}
