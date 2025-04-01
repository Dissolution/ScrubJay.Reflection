#pragma warning disable xUnit1026

#if !(NETFRAMEWORK || NETSTANDARD2_0)
using System.Runtime.CompilerServices;
#endif
using System.Reflection;

using TypeExtensions = ScrubJay.Reflection.Extensions.TypeExtensions;


namespace ScrubJay.Reflection.Tests.ExtensionsTests.TypeExtensionsTests;

public class UnmanagedTests
{
    public static TheoryData<object> UnmanagedObjects = new()
    {
        (object)(byte)4,
        (object)DateTime.Parse("1999-12-31 23:59:59"),
        (object)Guid.Parse("00000000-DEAD-BEEF-CAFE-000000000000"),
        (object)(BindingFlags.Public | BindingFlags.Static),
        (object)decimal.MaxValue,
        (object)"Sphinx of black quartz, judge my vow",
        (object)(new ValueTuple<int,int>(3,4)),

    };

#if !(NETFRAMEWORK || NETSTANDARD2_0)
    [Theory]
    [MemberData(nameof(UnmanagedObjects))]
    public void IsUnmanagedWorks<T>(T value)
    {
        bool notRefs = !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        bool isUnmanaged = TypeExtensions.IsUnmanaged(typeof(T));
        Assert.Equal(notRefs, isUnmanaged);
    }
#endif
}
