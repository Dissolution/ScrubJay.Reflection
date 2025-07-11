namespace ScrubJay.Reflection.Text;

public static class TextBuilderExtensions
{
    internal static TextBuilder NameGenericsParameters(
        this TextBuilder builder,
        string? name,
        Type[]? genericTypes = null,
        ParameterInfo[]? parameters = null)
    {
        if (name is not null)
        {
            int i = name.LastIndexOf('`');
            if (i >= 0)
            {
                builder.Append(name.AsSpan(0, i));
            }
            else
            {
                builder.Append(name);
            }
        }
        else
        {
            builder.Append("__???");
        }

        if (genericTypes is not null && genericTypes.Length > 0)
        {
            builder.Append('<')
                .EnumerateAndDelimit(genericTypes, static (tb, p) => tb.Render(p), ", ")
                .Append('>');
        }

        if (parameters is not null)
        {
            builder.Append('(')
                .EnumerateAndDelimit(parameters, static (tb, p) => tb.Render(p), ", ")
                .Append(')');
        }

        return builder;
    }
}