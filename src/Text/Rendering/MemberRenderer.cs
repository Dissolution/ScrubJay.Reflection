using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

public abstract class MemberRenderer<M> : Renderer<M>
    where M : MemberInfo
{
    protected virtual TextBuilder AppendAttributes(TextBuilder builder, M member)
    {
        return builder.If(Attribute.GetCustomAttributes(member, true), 
            static attrs => attrs.Length > 0,
            static (tb, attrs) => tb
                .Append('[')
                .Delimit(", ", attrs)
                .Append("] "));
    }

    protected virtual IEnumerable<string> GetModifiers(M member) => [];

    protected virtual TextBuilder AppendPreName(TextBuilder builder, M member) => builder;
    
    protected virtual TextBuilder AppendPostName(TextBuilder builder, M member) => builder;

    protected virtual void WriteAttributes(TextBuilder builder, M member, out bool typeNullable)
    {
        var conditions = Nullability.GetConditions(member);
        
        HashSet<Attribute> attributes = new();
        if (conditions.ReadAttr is not null)
            attributes.Add(conditions.ReadAttr);
        if (conditions.WriteAttr is not null)
            attributes.Add(conditions.WriteAttr);
        attributes.AddMany(member.GetAttributes());
        attributes.RemoveWhere(static attr => attr is NullableAttribute);

        builder.If(attributes,
            static attrs => attrs.Count > 0,
            static (tb, attrs) => tb
                .Append('[')
                .Delimit(", ", attrs)
                .Append("] "));
        typeNullable = conditions.TypeNullable;
    }
    
    public override TextBuilder RenderTo(TextBuilder builder, M? member)
    {
        if (member is null)
        {
            return builder.Append('(').Render(typeof(M)).Append(")null");
        }

        return builder
            .Invoke(member, AppendAttributes)
            .Render(member.Visibility)
            .Append(' ')
            .Enumerate(GetModifiers(member), static (tb, mod) => tb.Append(mod).Append(' '))
            .Invoke(member, AppendPreName)
            .Append(member.Name)
            .Invoke(member, AppendPostName);
    }
}