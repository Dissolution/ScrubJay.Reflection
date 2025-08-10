namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class PropertyRenderer : Renderer<PropertyInfo>
{
    public override void RenderTo(PropertyInfo? property, TextBuilder builder)
    {
        if (property is not null)
        {
            var (prefix, postfix) = property.NullabilityInfo().GetPrefixPostfix();

            builder
                .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
                .IfAppend(property.IsStatic(), "static ")
                .Render(property.PropertyType)
                .Append(postfix)
                .Append(' ')
                .Render(property.OwnerType())
                .Append('.')
                .Append(property.Name)
                .IfNotEmpty(
                    property.GetIndexParameters(),
                    static (tb, indexers) => tb
                        .Append('[')
                        .EnumerateAndDelimit(indexers, static (tb, i) => tb.Render(i), ", ")
                        .Append(']'));
                    
        }
    }
}