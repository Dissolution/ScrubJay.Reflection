using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class ConstructorRenderer : Renderer<ConstructorInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, ConstructorInfo? ctor)
    {
        if (ctor is not null)
        {
            Type constructedType = ctor.DeclaringType.ThrowIfNull("Constructor has null Declaring Type");
            string name = constructedType.Name;
            if (name == ".ctor")
            {
                builder.Append("new ")
                    .Render(constructedType);
            }
            else if (name == ".cctor")
            {
                builder.Append("static ")
                    .Render(constructedType);
            }
            else
            {
                builder.Append("new ")
                    .Render(constructedType);
            }

            if (ctor.IsGenericMethod)
                Debugger.Break();

            return builder.Append('(')
                .Delimit(", ", ctor.GetParameters())
                .Append(')');
        }

        return builder;
    }
}