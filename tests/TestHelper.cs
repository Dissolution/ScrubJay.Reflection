using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Tests;

internal static class TestHelper
{
    public static void AssertPropertiesEqual<T>(T? left, T? right)
    {
        Assert.Equal<bool>(left is null, right is null);
        if (left is null) return;

        var properties = Mirror.For<T>()
            .Properties.Public.Instance
            .NoIndexers
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