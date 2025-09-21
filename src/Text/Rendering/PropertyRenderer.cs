using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class PropertyRenderer : MemberRenderer<PropertyInfo>, IRenderer
{
    public override TextBuilder RenderTo(TextBuilder builder, PropertyInfo? property)
    {
        if (property is null)
            return builder;

        WriteAttributes(builder, property, out var typeNullable);
        
        builder
            .Render(property.Visibility)
            .Append(' ')
            .Render(property.PropertyType)
            .If(typeNullable, '?')
            .Append(' ')
            .Append(property.Name)
            .IfNotEmpty(property.GetIndexParameters(), (tb, indexers) => tb
                .Append('[')
                .Delimit(", ", indexers)
                .Append(']'));

        var visibility = property.Visibility;
        var getter = property.GetMethod;
        var setter = property.SetMethod;
        if (getter is not null || setter is not null)
        {
            builder.Append(" { ");

            if (getter is not null)
            {
                builder.If(getter.Visibility,
                        vis => vis != visibility,
                        static (tb, vis) => tb.Render(vis).Write(' '))
                    .Append("get; ");
            }

            if (setter is not null)
            {
                if (setter.Visibility != visibility)
                {
                    builder.Render(setter.Visibility)
                        .Write(' ');
                }

                // `init` properties have a special custom modifier
                if (setter.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Contains(typeof(IsExternalInit)))
                {
                    builder.Append("init; ");
                }
                else
                {
                    builder.Append("set; ");
                }
            }

            builder.Append('}');
        }

        return builder;
    }
}