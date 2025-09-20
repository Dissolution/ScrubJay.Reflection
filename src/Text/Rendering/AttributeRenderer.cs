using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class AttributeRenderer : Renderer<Attribute>
{
    public override TextBuilder RenderTo(TextBuilder builder, Attribute? attribute)
    {
        if (attribute is null)
            return builder;
        // todo
        return builder.Format(attribute);
    }
}