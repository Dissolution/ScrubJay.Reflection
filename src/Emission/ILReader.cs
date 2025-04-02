using System.Reflection.Emit;
using Microsoft.VisualBasic;

namespace ScrubJay.Reflection.Emission;





public ref struct ILReader
{
    private static readonly OpCode[] _singleByteOpCodes;
    private static readonly OpCode[] _doubleByteOpCodes;
    
    static ILReader()
    {
        _singleByteOpCodes = new OpCode[225];
        _doubleByteOpCodes = new OpCode[31];

        var fields = typeof(OpCodes).GetFields(BF.Public | BF.Static);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType != typeof(OpCode))
                continue;
            OpCode opCode = field.GetValue(null).ThrowIfNot<OpCode>();
            if (opCode.OpCodeType == OpCodeType.Nternal)
                continue;

            if (opCode.Size == 1)
            {
                Debug.Assert(opCode.Value < 225);
                _singleByteOpCodes[opCode.Value] = opCode;
            }
            else
            {
                Debug.Assert(opCode.Size == 2);
                Debug.Assert((opCode.Value & 0xFF) < 31);
                _doubleByteOpCodes[opCode.Value & 0xFF] = opCode;
            }
        }

        Debug.Assert(_singleByteOpCodes.All(oc => oc != default));
        Debug.Assert(_doubleByteOpCodes.All(oc => oc != default));
    }
        
    
    private readonly ReadOnlySpan<byte> _ilBytes;
    private          int                _position;
    
    public int Offset
    {
        get => _position;
        set => _position = value.Clamp(0, Length);
    }

    public readonly int Length => _ilBytes.Length;
    public readonly bool HasMore => _position < Length;

    public ILReader(ReadOnlySpan<byte> ilBytes, int position = 0)
    {
        _ilBytes = ilBytes;
        _position = position;
    }
    
    public Option<U> Peek<U>(int offset = 0)
        where U : unmanaged
    {
        int size = Notsafe.SizeOf<U>();
        Debug.Assert(size > 0);
        int pos = _position;
        if (pos + offset + size > Length)
            return None<U>();
        
        var slice = _ilBytes.Slice(pos + offset, size);
        return Some(Unsafe.ReadUnaligned<U>(in slice.GetPinnableReference()));
    }

    public Option<U> Read<U>()
        where U : unmanaged
    {
        int size = Notsafe.SizeOf<U>();
        Debug.Assert(size > 0);
        int pos = _position;
        int newPos = pos + size;
        if (newPos > Length)
            return None<U>();

        _position = newPos;
        var slice = _ilBytes.Slice(pos, size);
        return Some(Unsafe.ReadUnaligned<U>(in slice.GetPinnableReference()));
    }

    public Option<OpCode> PeekOpCode()
    {
        if (!Peek<byte>().IsSome(out byte code))
            return None();

        if (code == 254) // 2-byte opcode
        {
            if (!Peek<byte>(1).IsSome(out code))
                return None();

            return Some(_doubleByteOpCodes[code]);
        }
        else
        {
            return Some(_singleByteOpCodes[code]);
        }
    }
    
    public Option<OpCode> ReadOpCode()
    {
        if (!Read<byte>().IsSome(out byte code))
            return None<OpCode>();

        if (code == 255)
        {
            Debugger.Break();
            return None<OpCode>();
        }
        
        if (code < 254) // single-byte opcode
        {
            if (code < 225)
            {
                return Some(_singleByteOpCodes[code]);
            }
            Debugger.Break();
            return None<OpCode>();
        }
        else // double-byte opcode
        {
            if (!Read<byte>().IsSome(out code))
                return None();

            if (code < 31)
            {
                return Some(_doubleByteOpCodes[code]);
            }
            Debugger.Break();
            return None<OpCode>();
        }
    }



    public OpCode OpCode { get; private set; }
    public int MetadataToken { get; private set; }
    public MemberInfo? Operand { get; private set; }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode(Constants.TrimWarning)]
#endif
    public bool Read(MethodBase methodInfo)
    {
        if (ptr < _cil.Length)
        {
            OpCode = ReadOpCode();
            Operand = ReadOperand(OpCode, methodInfo);
            return true;
        }
        return false;
    }



#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode(Constants.TrimWarning)]
#endif
    MemberInfo? ReadOperand(OpCode code, MethodBase methodInfo)
    {
        MetadataToken = 0;
        int inlineLength;
        switch (code.OperandType)
        {
            case OperandType.InlineMethod:
                MetadataToken = ReadInt();
                Type[]? methodArgs = null;
                if (methodInfo.GetType() != typeof(ConstructorInfo) && !methodInfo.GetType().IsSubclassOf(typeof(ConstructorInfo)))
                {
                    methodArgs = methodInfo.GetGenericArguments();
                }
                Type[]? typeArgs = null;
                if (methodInfo.DeclaringType != null)
                {
                    typeArgs = methodInfo.DeclaringType.GetGenericArguments();
                }
                try
                {
                    return methodInfo.Module.ResolveMember(MetadataToken, typeArgs, methodArgs);
                }
                catch
                {
                    // Can return System.ArgumentException : Token xxx is not a valid MemberInfo token in the scope of module xxx.dll
                    return null;
                }

            case OperandType.InlineNone:
                inlineLength = 0;
                break;

            case OperandType.ShortInlineBrTarget:
            case OperandType.ShortInlineVar:
            case OperandType.ShortInlineI:
                inlineLength = 1;
                break;

            case OperandType.InlineVar:
                inlineLength = 2;
                break;

            case OperandType.InlineBrTarget:
            case OperandType.InlineField:
            case OperandType.InlineI:
            case OperandType.InlineString:
            case OperandType.InlineSig:
            case OperandType.InlineSwitch:
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.ShortInlineR:
                inlineLength = 4;
                break;

            case OperandType.InlineI8:
            case OperandType.InlineR:
                inlineLength = 8;
                break;

            default:
                return null;
        }

        for (var i = 0; i < inlineLength; i++)
        {
            ReadByte();
        }

        return null;
    }

  

      

}