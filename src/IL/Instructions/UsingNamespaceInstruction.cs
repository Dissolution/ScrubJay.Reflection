namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class UsingNamespaceInstruction : ILGeneratorInstruction
{
    public string Namespace { get; }
    
    public UsingNamespaceInstruction(string @namespace) 
        : base(ILGeneratorMethod.UsingNamespace)
    {
        this.Namespace = @namespace;
        // todo: validate namespace?
    }

    protected internal override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Append('"').Append(Namespace).Append('"');
    }
}