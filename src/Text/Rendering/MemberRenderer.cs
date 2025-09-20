using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

public abstract class MemberRenderer<M> : Renderer<M>
    where M : MemberInfo
{
    protected virtual IEnumerable<string> GetModifiers(M member) => [];
    
    protected virtual TextBuilder AppendPreName(TextBuilder builder, M member) => builder;
    protected virtual TextBuilder AppendPostName(TextBuilder builder, M member) => builder;

    protected MemberRenderer()
    {
    }

    public sealed override TextBuilder RenderTo(TextBuilder builder, M? member)
    {
        if (member is null)
        {
            return builder.Append('(').Render(typeof(M)).Append(")null");
        }

        var attributes = Attribute.GetCustomAttributes(member);
        if (attributes.Length > 0)
        {
            builder.Append('[')
                .Delimit(", ", attributes)
                .Append("] ");
        }

        return builder
            .Render(member.Visibility)
            .Append(' ')
            .Enumerate(GetModifiers(member), static (tb,mod) => tb.Append(mod).Append(' '))
            .Invoke(member, AppendPreName)
            .Append(member.Name)
            .Invoke(member, AppendPostName);
    }
}