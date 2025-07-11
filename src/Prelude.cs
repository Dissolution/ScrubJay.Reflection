#pragma warning disable CS8981

global using BF = System.Reflection.BindingFlags;
global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using text = System.ReadOnlySpan<char>;

using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection;

[PublicAPI]
public static class Prelude
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Mirror Reflect(Type type) => Mirror.Reflect(type);
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Mirror<T> Reflect<T>() => Mirror.Reflect<T>();
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Mirror ReflectOn(object obj) => Reflect(obj.GetType());
}