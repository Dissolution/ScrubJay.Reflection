using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class PropertyRenderer : MemberRenderer<PropertyInfo>, IRenderer
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

    protected override TextBuilder AppendAttributes(TextBuilder builder, PropertyInfo property)
    {
        var attributes = Attribute.GetCustomAttributes(property, true);
        var (precondition, postcondition, typeQ) = Nullability.GetConditions(property);
    
        Debugger.Break();
        
        if (attributes.Length > 0 || precondition is not null || postcondition is not null)
        {
            builder.Append('[');
    
            if (precondition is not null)
            {
                builder.Append(precondition);
                if (postcondition is not null || attributes.Length > 0)
                {
                    builder.Append(", ");
                }
            }
    
            if (postcondition is not null)
            {
                builder.Append(postcondition);
                if (attributes.Length > 0)
                {
                    builder.Append(", ");
                }
            }
    
            return builder.Delimit(", ", attributes).Append("] ");
        }
    
        
        return builder;
    }

    protected override IEnumerable<string> GetModifiers(PropertyInfo member)
    {
        // ?
        return [];
    }

    protected override TextBuilder AppendPreName(TextBuilder builder, PropertyInfo property)
    {
        var pt = property.PropertyType;
        builder.Render(pt);

        var pta = Attribute.GetCustomAttributes(pt);
        if (pta.TryGet<NullableContextAttribute>(out var attr))
        {
            NullableContext context = (NullableContext)attr.Flag;
            if (context != NullableContext.Annotated)
            {
                Debugger.Break();
                builder.Append('?');
            }
        }

        return builder.Append(' ');
    }
    
    protected override TextBuilder AppendPostName(TextBuilder builder, PropertyInfo property)
    {
        var visibility = property.Visibility;
        var getter = property.GetMethod;
        var setter = property.SetMethod;
        if (getter is not null || setter is not null)
        {
            builder.Append(" { ");
            
            if (getter is not null)
            {
                if (getter.Visibility != visibility)
                {
                    builder.Render(getter.Visibility)
                        .Write(' ');
                }
                
                builder.Append("get; ");
            }

            if (setter is not null)
            {
                if (setter.Visibility != visibility)
                {
                    builder.Render(setter.Visibility)
                        .Write(' ');
                }
                
                // `init` properties have a special custom modifier
                if (setter.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Contains(typeof(IsExternalInit)))
                {
                    builder.Append("init; ");
                }
                else
                {
                    builder.Append("set; ");
                }
            }

            builder.Append('}');
        }

        return builder;
    }
}