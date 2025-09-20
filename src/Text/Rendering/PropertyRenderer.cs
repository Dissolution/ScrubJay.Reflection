using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class PropertyRenderer : MemberRenderer<PropertyInfo>
{
    // public override TextBuilder RenderTo(TextBuilder builder, PropertyInfo? property)
    // {
    //     if (property is null)
    //         return builder;
    //
    //     var (prefix, postfix) = Nullability.Get(property).GetPrefixPostfix();
    //
    //     return builder
    //         .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
    //         .If(property.IsStatic(), "static ")
    //         .Render(property.PropertyType)
    //         .Append(postfix)
    //         .Append(' ')
    //         .Render(property.OwnerType)
    //         .Append('.')
    //         .Append(property.Name)
    //         .IfNotEmpty(
    //             property.GetIndexParameters(),
    //             static (tb, indexers) => tb
    //                 .Append('[')
    //                 .Delimit(", ", indexers)
    //                 .Append(']'));
    // }

    public PropertyRenderer()
    {
    }

    protected override IEnumerable<string> GetModifiers(PropertyInfo member)
    {
        // ?
        return [];
    }

    protected override TextBuilder AppendPreName(TextBuilder builder, PropertyInfo property)
    {
        var pt = property.PropertyType;
        var pta = Attribute.GetCustomAttributes(pt);
        Debugger.Break();
        throw new NotImplementedException();
    }

    protected override TextBuilder AppendPostName(TextBuilder builder, PropertyInfo member)
    {
        throw new NotImplementedException();
    }
}