#pragma warning disable CS8981

// alias `ReadOnlySpan<char>` to `text`
global using text = System.ReadOnlySpan<char>;
// prevent attribute name conflict with `JetBrains.Annotations.NotNullAttribute`
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

global using Viz = ScrubJay.Reflection.Visibility;
global using TRK = ScrubJay.Reflection.TypeRefKind;

namespace ScrubJay.Reflection;

[PublicAPI]
public static class Prelude
{
    
}