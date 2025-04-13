using System.Runtime.CompilerServices;
using ScrubJay.Reflection.Extensions;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Tests;

public class TypeHelperTests
{
    public static TheoryData<object> UnmanagedObjects { get; } = new(TestData.UnmanagedObjects);
    
    [Theory]
    [MemberData(nameof(UnmanagedObjects))]
    public void IsUnmanagedWorks<T>(T value)
    {
        bool notRefs = !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        bool isUnmanaged = TypeHelper.IsUnmanaged<T>();
        Assert.Equal(notRefs, isUnmanaged);
    }
}