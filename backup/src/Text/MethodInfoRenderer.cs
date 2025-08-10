namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class MethodInfoRenderer : Renderer<MethodInfo>
{
    public override void RenderTo(MethodInfo? method, TextBuilder builder)
    {
        if (method is null) return;

        builder
            .Render(method.Visibility())
            .Append(' ')
            .IfAppend(method.IsStatic, "static ")
            .IfAppend(method.IsAsync(), "async ")
            .IfNotNull(method.ReturnParameter,
                static (tb, p) => tb.Render(p),
                tb => tb.Render(method.ReturnType))
            .Append(' ')
            .Render(method.OwnerType())
            .Append('.')
            .NameGenericsParameters(
                method.Name,
                method.GetGenericArguments(),
                method.GetParameters());
    }
}