using ScrubJay.Collections;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class FieldRenderer : MemberRenderer<FieldInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, FieldInfo? field)
    {
        if (field is null)
            return builder;

        WriteAttributes(builder, field, out var typeNullable);

        return builder
            .Render(field.Visibility)
            .Append(' ')
            .If(field.IsLiteral, "const ")
            .If(field.IsInitOnly, "readonly ")
            .Render(field.FieldType)
            .If(typeNullable, '?')
            .Append(' ')
            .Append(field.Name);
    }
}