namespace ScrubJay.Reflection.Extensions;

public static class ObjectExtensions
{

    public static bool CanBeA(this object? input, Type? type)
    {
        if (input is null)
            return type.CanContainNull();
        return input.GetType().Implements(type);
    }
}