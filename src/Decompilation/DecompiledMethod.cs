#pragma warning disable CS0618

using ScrubJay.Memory;

namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public sealed class DecompiledMethod
{
    public Module? Module { get; }
    public Type? DeclaringType { get; }
    public Type[]? DeclarerGenericTypes { get; }
    public MethodBase Method { get; }
    public MethodBody? Body { get; }
    public ParameterInfo[]? Parameters { get; }
    public Type[]? ParameterTypes { get; }
    public ParameterInfo? Return { get; }
    public Type[]? MethodGenericTypes { get; }
    public LocalVariableInfo[]? Locals { get; }
    public byte[]? ILBytes { get; }
    public InstructionStream Instructions { get; } = [];
    public IMemberMetadataResolver ModuleMemberResolver { get; }

    public DecompiledMethod(MethodBase method)
    {
        Method = method.ThrowIfNull();
        Module = method.Module;
        MethodGenericTypes = Method.GetGenericArguments();
        DeclaringType = method.DeclaringType;
        DeclarerGenericTypes = DeclaringType?.GetGenericArguments();

        ModuleMemberResolver = new ModuleMemberMetadataResolver(Module, DeclarerGenericTypes, MethodGenericTypes);

        if (method.IsStatic)
        {
            Parameters = method.GetParameters();
        }
        else
        {
            Parameters = [new ThisParameterInfo(Method), ..method.GetParameters()];
        }

        ParameterTypes = Parameters.ConvertAll(static p => p.ParameterType);

        if (method is MethodInfo methodInfo)
        {
            Return = methodInfo.ReturnParameter;
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            Return = new ReturnParameterInfo(constructorInfo);
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        Body = method.GetMethodBody();
        if (Body is not null)
        {
            Locals = [..Body.LocalVariables];
            ILBytes = Body.GetILAsByteArray();
        }
        else
        {
            Locals = null;
            ILBytes = null;
        }
        
        // read instructions if we can
        ReadInstructions();
    }

    private void ReadInstructions()
    {
        if (ILBytes is null)
            return;

        byte[] ilBytes = ILBytes;
        var reader = new SpanReader<byte>(ilBytes);
        
        while (!reader.IsCompleted)
        {
            ILOffset offset = reader.Position;
            OpCode opCode = OpCodeHelper.ReadOpCode(ref reader);
            Option<object?> operand = ReadOperand(ref reader, opCode);
            Instruction instruction = new(offset, opCode, operand);
            Instructions.Add(instruction);
        }
    }

    private object ReadVariable(OpCode opCode, int index)
    {
        if (opCode.Name!.Contains("loc"))
            return Locals![index];
        return Parameters![index];
    }

    private Option<object?> ReadOperand(ref SpanReader<byte> reader, OpCode opCode)
    {
        var operandType = opCode.OperandType;
        switch (operandType)
        {
            case OperandType.ShortInlineI:
            {
                if (opCode == OpCodes.Ldc_I4_S)
                {
                    sbyte i8 = reader.ReadI8();
                    return Some<object?>(i8);
                }
                else
                {
                    byte u8 = reader.ReadU8();
                    return Some<object?>(u8);
                }
            }
            case OperandType.InlineI:
            {
                int i32 = reader.ReadI32();
                return Some<object?>(i32);
            }
            case OperandType.InlineI8:
            {
                long i64 = reader.ReadI64();
                return Some<object?>(i64);
            }
            case OperandType.ShortInlineR:
            {
                float f32 = reader.ReadF32();
                return Some<object?>(f32);
            }
            case OperandType.InlineR:
            {
                double f64 = reader.ReadF64();
                return Some<object?>(f64);
            }
            
            // ---
            case OperandType.ShortInlineBrTarget:
            {
                ILOffset offset = (reader.ReadI8() + reader.Position);
                return Some<object?>(offset);
            }
            case OperandType.InlineBrTarget:
            {
                ILOffset offset = (reader.ReadI32() + reader.Position);
                return Some<object?>(offset);
            }
            case OperandType.InlineSwitch:
            {
                int length = reader.ReadI32();
                int baseOffset = reader.Position + (4 * length);
                ILOffset[] branches = new ILOffset[length];
                for (int i = 0; i < length; i++)
                {
                    branches[i] = (reader.ReadI32() + baseOffset);
                }

                return Some<object?>(branches);
            }
            
            // ---
            
            case OperandType.InlineField:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                FieldInfo field = ModuleMemberResolver.TryResolveField(metadataToken).OkOrThrow();
                return Some<object?>(field);
            }
            case OperandType.InlineMethod:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                MethodBase method = ModuleMemberResolver.TryResolveMethod(metadataToken).OkOrThrow();
                return Some<object?>(method);
            }
            case OperandType.InlineType:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                Type type = ModuleMemberResolver.TryResolveType(metadataToken).OkOrThrow();
                return Some<object?>(type);
            }
            case OperandType.InlineTok:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                MemberInfo member = ModuleMemberResolver.TryResolveMember(metadataToken).OkOrThrow();
                return Some<object?>(member);
            }
            case OperandType.InlineSig:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                byte[] signature = ModuleMemberResolver.TryResolveSignature(metadataToken).OkOrThrow();
                return Some<object?>(signature);
            }
            case OperandType.InlineString:
            {
                MetadataToken metadataToken = reader.ReadUnmanaged<MetadataToken>();
                string str = ModuleMemberResolver.TryResolveString(metadataToken).OkOrThrow();
                return Some<object?>(str);
            }
           
            // ---
            case OperandType.ShortInlineVar:
            {
                sbyte index = reader.ReadI8();
                object var = ReadVariable(opCode, index);
                return Some<object?>(var);
            }
            case OperandType.InlineVar:
            {
                short index = reader.ReadI16();
                object var = ReadVariable(opCode, index);
                return Some<object?>(var);
            } case OperandType.InlineNone:
            case OperandType.InlinePhi:
            {
                return None;
            }
            default:
                throw InvalidEnumException.New(operandType);
        }
    }

    public override string ToString()
    {
        return TextBuilder.New
            .Render(Module)
            .Append("::")
            .Render(DeclaringType)
            .Append("  ")
            .Render(Method)
            .NewLine()
            .IfNotEmpty(Locals, static (tb, locals) => tb
                .Append("Locals:")
                .Indent()
                .NewLine()
                .Delimit(Delimiter.NewLine, locals)
                .Dedent()
                .NewLine())
            .IfNotEmpty(Instructions, static (tb, instructions) => tb
                .Append("Instructions:")
                .Indent()
                .NewLine()
                .Delimit(Delimiter.NewLine, instructions)
                .Dedent()
                .NewLine())
            .ToStringAndDispose();
    }
}