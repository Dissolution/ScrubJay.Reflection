using ScrubJay.Reflection.Cloning;
using ScrubJay.Reflection.Tests.ExtensionsTests.TypeExtensionsTests;

namespace ScrubJay.Reflection.Tests.CloningTests;

public class DeepCloneTests
{
    public static TheoryData<object> UnmanagedObjects => UnmanagedTests.UnmanagedObjects;

    public static TheoryData<object> ManagedObjects { get; } = new()
    {
        (object)(new Tuple<int, string>(147, "TJ")),
        (object)(new int[]{1,4,7}),
        (object)(new List<char>("abc".ToCharArray())),
        (object)(new TestRecord(147,"TJ")),
        //(object)(new StringBuilder().Append("TRJ")),
    };

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

    [Theory]
    [MemberData(nameof(UnmanagedObjects))]
    public void CanDeepCloneUnmanaged<T>(T nmanaged)
    {
        var clone = Cloner.DeepClone<T>(nmanaged);
        Assert.Equal<T>(nmanaged, clone);
    }
    
    [Theory]
    [MemberData(nameof(ManagedObjects))]
    public void CanDeepCloneManagedEquatable<T>(T managed)
    {
        var clone = Cloner.DeepClone<T>(managed);
        Assert.False(ReferenceEquals(managed, clone));
        TestHelper.AssertPropertiesEqual(managed, clone);
    }
    
    
    
}
