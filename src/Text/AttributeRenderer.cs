namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class AttributeRenderer : Renderer<Attribute>
{
    public override void RenderTo(Attribute? attribute, TextBuilder builder)
    {
        if (attribute is null) return;
        // todo
        builder.Format(attribute);
    }
}