namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TextBuilderExtensions
{
    internal static TextBuilder RenderGenericTypes(this TextBuilder builder, params Type[]? genericTypes)
    {
        return builder.IfNotEmpty(genericTypes,
            static (tb, gts) => tb
                .Append('<')
                .EnumerateAndDelimit(gts, static (t, g) => t.Render(g), ", ")
                .Append('>'));
    }
}