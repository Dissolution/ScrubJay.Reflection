using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class Instruction : IRenderable
{
    public ILOffset Offset { get; internal set; } = ILOffset.Unknown;
    
    public abstract int Size { get; }

    protected internal Instruction() { }
    
    public override sealed int GetHashCode()
        => Throw.NotSupported<int>($"An {GetType().NameOf()} should only be stored in an {typeof(InstructionStream).NameOf()}");

    public virtual void RenderTo<B>(B builder) 
        where B : TextBuilderBase<B>
    {
        builder.Render(Offset).Append(": ");
    }

    public override sealed string ToString() => TextBuilder.Build(RenderTo);
}