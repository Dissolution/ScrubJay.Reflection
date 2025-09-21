using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class ParameterRenderer : Renderer<ParameterInfo>
{
    private void WriteAttributes(TextBuilder builder, ParameterInfo parameter, out bool typeNullable)
    {
        var conditions = Nullability.GetConditions(parameter);
        
        HashSet<Attribute> attributes = new();
        if (conditions.ReadAttr is not null)
            attributes.Add(conditions.ReadAttr);
        if (conditions.WriteAttr is not null)
            attributes.Add(conditions.WriteAttr);
        attributes.AddMany(parameter.GetAttributes());
        attributes.RemoveWhere(static attr => attr is NullableAttribute);

        builder.If(attributes,
            static attrs => attrs.Count > 0,
            static (tb, attrs) => tb
                .Append('[')
                .Delimit(", ", attrs)
                .Append("] "));
        typeNullable = conditions.TypeNullable;
    }
    
    public override TextBuilder RenderTo(TextBuilder builder, ParameterInfo? parameter)
    {
        if (parameter is null)
            return builder;

        WriteAttributes(builder, parameter, out var typeNullable);
        
        (TRK paramRef, Type paramType) = parameter;

        return builder.Render(paramRef)
            .Render(paramType)
            .If(typeNullable, '?')
            .IfNotEmpty(parameter.Name, static (tb, name) => tb.Append(' ').Append(name))
            .If(parameter.Default,
                static (tb, defaultValue) => tb.Append(" = ").Render(defaultValue));
    }
}