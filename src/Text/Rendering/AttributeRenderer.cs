using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class AttributeRenderer : Renderer<Attribute>
{
    public override TextBuilder RenderTo(TextBuilder builder, Attribute? attribute)
    {
        if (attribute is null)
            return builder;

        var attrType = attribute.GetType();

        // Add the attribute name
        builder.Render(attrType);
        // but remove the 'Attribute' part
        if (builder.Written.EndsWith("Attribute"))
        {
            builder.Length -= 9;
        }

        var props =
            attrType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(static prop => prop.Name != nameof(Attribute.TypeId))
                .ToList();

        if (props.Count > 0)
        {
            return builder.Append('(')
                .Delimit(", ", props)
                .Append(')');
        }
        //
        // var fields = attrType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        //     .ToList();
        //
        // if (fields.Count > 0)
        // {
        //     return builder.Append('(')
        //         .Delimit(", ", fields, (tb,field) => tb.Append(field.Name).Append(" = ").Render(field.GetValue(attribute)))
        //         .Append(')');
        // }
        //
        // Debugger.Break();
        return builder;
    }
}