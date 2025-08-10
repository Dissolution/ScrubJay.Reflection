namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class TokenInstruction : OpCodeInstruction
{
    public int Token { get; }

    public override sealed int Size => OpCode.Size + sizeof(int);

    internal protected TokenInstruction(OpCode opCode, int token)
        : base(opCode)
    {
        this.Token = token;
    }
}