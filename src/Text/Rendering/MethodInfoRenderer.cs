using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class MethodInfoRenderer : MemberRenderer<MethodInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, MethodInfo? method)
    {
        if (method is null)
            return builder;

        WriteAttributes(builder, method, out var typeNullable);

        return builder
            .Render(method.Visibility)
            .Append(' ')
            .If(method.IsStatic, "static ")
            .If(method.IsAsync, "async ")
            .IfNotNull(method.ReturnParameter,
                static (tb, p) => tb.Render(p),
                tb => tb.Render(method.ReturnType).If(typeNullable, '?'))
            .Append(' ')
            .Render(method.OwnerType)
            .Append('.')
            .AppendNameGenericsAndParameters(method);
    }
}