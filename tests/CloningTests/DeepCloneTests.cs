//using ScrubJay.Reflection.Cloning;
//
//namespace ScrubJay.Reflection.Tests.CloningTests;
//
//public class DeepCloneTests
//{
//    public static TheoryData<object> UnmanagedObjects { get; } = new(TestData.UnmanagedObjects);
//
//    public static TheoryData<object> ManagedObjects { get; } = new(TestData.ManagedObjects);
//
//    [Fact]
//    public void CanDeepCloneNull()
//    {
//        var clone = Cloner.DeepClone<object>(null);
//        Assert.Null(clone);
//
//        clone = Cloner.DeepClone<string>(null);
//        Assert.Null(clone);
//
//        clone = Cloner.DeepClone<int?>(null);
//        Assert.Null(clone);
//    }
//
//    [Theory]
//    [MemberData(nameof(UnmanagedObjects))]
//    public void CanDeepCloneUnmanaged<T>(T nmanaged)
//        where T : unmanaged
//    {
//        var clone = Cloner.DeepClone<T>(nmanaged);
//        AssertDeeper.AssertDeepEqual<T>(nmanaged, clone);
//    }
//    
//    [Theory]
//    [MemberData(nameof(ManagedObjects))]
//    public void CanDeepCloneManagedEquatable<T>(T managed)
//    {
//        var clone = Cloner.DeepClone<T>(managed);
//        Assert.False(ReferenceEquals(managed, clone));
//        AssertDeeper.AssertDeepEqual<T>(managed, clone);
//    }
//    
//    
//    
//}
