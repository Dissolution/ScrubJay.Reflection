global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection;

[PublicAPI]
public static class Prelude
{
    public static Mirror Mirror(Type type)
    {
        Throw.IfNull(type);
        return new Mirror(type);
    }

    public static Mirror<T> Mirror<T>() => new();
}