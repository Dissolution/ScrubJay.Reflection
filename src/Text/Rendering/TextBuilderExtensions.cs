namespace ScrubJay.Reflection.Text.Rendering;

internal static class TextBuilderExtensions
{
    public static TextBuilder NameGenericsParameters(
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
                .Delimit(", ", genericTypes)
                .Append('>');
        }

        if (parameters is not null)
        {
            builder.Append('(')
                .Delimit(", ", parameters)
                .Append(')');
        }

        return builder;
    }
}