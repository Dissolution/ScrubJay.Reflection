using ScrubJay.Collections;

namespace ScrubJay.Reflection.Text.Rendering;

/// <summary>
/// Renders a <see cref="FieldInfo"/> instance
/// </summary>
/// <remarks>
/// We want to render a FieldInfo into nearly the exact representation one would see in C#
/// [Attr1, Attr2] visibility mrk type name
///
/// </remarks>
[PublicAPI]
public sealed class FieldRenderer : MemberRenderer<FieldInfo>
{
    // public override TextBuilder RenderTo(TextBuilder builder, FieldInfo? field)
    // {
    //     if (field is null)
    //         return builder;
    //
    //     var (prefix, postfix) = Nullability.Get(field).GetPrefixPostfix();
    //
    //     return builder
    //         .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
    //         .If(field.IsStatic, "static ")
    //         .Render(field.FieldType)
    //         .Append(postfix)
    //         .Append(' ')
    //         .Render(field.OwnerType)
    //         .Append('.')
    //         .Append(field.Name);
    // }

    protected override IEnumerable<string> GetModifiers(FieldInfo field)
    {
        if (field.IsLiteral)
            yield return "const";
        if (field.IsInitOnly)
            yield return "readonly";
    }

    protected override TextBuilder AppendPreName(TextBuilder builder, FieldInfo field)
    {
        throw new NotImplementedException();
    }

    protected override TextBuilder AppendPostName(TextBuilder builder, FieldInfo field)
    {
        throw new NotImplementedException();
    }
}