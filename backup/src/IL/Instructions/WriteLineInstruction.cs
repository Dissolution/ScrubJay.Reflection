namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class WriteLineInstruction : ILGeneratorInstruction
{
    public object? Object { get; }
    
    public WriteLineInstruction(object? obj) 
        : base(ILGeneratorMethod.WriteLine)
    {
        this.Object = obj;
    }

    public WriteLineInstruction(string? str) : base(ILGeneratorMethod.WriteLine)
    {
        this.Object = str;
    }

    public WriteLineInstruction(ILLocal local) : base(ILGeneratorMethod.WriteLine)
    {
        this.Object = local;
    }
    

    protected internal override TextBuilder RenderArgs(TextBuilder builder)
    {
        if (Object.Is<string>(out var str))
        {
            return builder.Append('"').Append(str).Append('"');
        }
        else if (Object.Is<ILLocal>(out var local))
        {
            return builder.Render(local);
        }
        else
        {
            Debugger.Break();
            return builder.Render(Object);
        }
    }
}
