namespace ScrubJay.Reflection.Text;

[PublicAPI]
public interface IRenderable
{
    void RenderTo<B>(B builder)
        where B : TextBuilderBase<B>;
}