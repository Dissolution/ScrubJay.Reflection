namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class FieldRenderer : Renderer<FieldInfo>
{
    public override void RenderTo(FieldInfo? field, TextBuilder builder)
    {
        if (field is not null)
        {
            var (prefix, postfix) = field.NullabilityInfo().GetPrefixPostfix();

            builder
                .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
                .IfAppend(field.IsStatic, "static ")
                .Render(field.FieldType)
                .Append(postfix)
                .Append(' ')
                .Render(field.OwnerType())
                .Append('.')
                .Append(field.Name);
        }
    }
}