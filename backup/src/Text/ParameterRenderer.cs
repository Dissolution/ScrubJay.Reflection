namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class ParameterRenderer : Renderer<ParameterInfo>
{
    public override void RenderTo(ParameterInfo? parameter, TextBuilder builder)
    {
        if (parameter is null) return;

        (TRK paramRef, Type paramType) = parameter;
        (string? prefix, string? postfix) = parameter.NullabilityInfo().GetPrefixPostfix();
        
        builder
            .IfNotEmpty(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .Append(paramRef.AsString())
            .Render(paramType)
            .Append(postfix)
            .IfNotEmpty(parameter.Name, static (tb, name) => tb.Append(' ').Append(name))
            .If(parameter.Default(),
                static (tb, defaultValue) => tb.Append(" = ").Render(defaultValue));
    }
}