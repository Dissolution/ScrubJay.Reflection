using ScrubJay.Enums;
using ScrubJay.Extensions;
using ScrubJay.Memory;
using ScrubJay.Reflection.Exceptions;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Decompilation;

public class ILDecompiler
{
    private static Result<IReadOnlyList<Instruction>> TryReadInstructions(
        scoped ReadOnlySpan<byte> bytes,
        Type[]? genericTypeArgs,
        Type[]? genericMethodArgs,
        Module module)
    {
        var instructions = new List<Instruction>();
        var reader = new SpanReader<byte>(bytes);

        byte u8;
        
        while (!reader.IsCompleted)
        {
            if (!TryReadOpCode(ref reader).IsOkWithError(out var opCode, out var error))
                return error;
            
            
        }

    }

    private static Result<OpCode> TryReadOpCode(ref SpanReader<byte> reader)
    {
        OpCode opCode;
        byte u8;
        
        if (!reader.TryTake().IsSome(out u8))
            return new ReflectionException($"Could not read first OpCode byte");

        if (u8 != 0xFE)
        {
            opCode = OpCodeHelper.OneByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                return new ReflectionException($"Invalid one-byte OpCode for 0x{u8:X}");
        }
        else
        {
            if (!reader.TryTake().IsSome(out u8))
                return new ReflectionException($"Could not read second OpCode byte");
            
            opCode = OpCodeHelper.TwoByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                return new ReflectionException($"Invalid two-byte OpCode for 0x{u8:X}");
        }

        return opCode;
    }
    
    
    public static Result<dynamic> Decompile(MethodBase method)
    {
        if (method is null)
            return new ArgumentNullException(nameof(method));

        if (!method.MethodImplementationFlags.HasFlags(MethodImplAttributes.IL))
            return new ArgumentException("Method is not implemented in IL", nameof(method));

        var methodBody = method.GetMethodBody();
        if (methodBody is null)
            return new ArgumentException("Could not retrieve method body", nameof(method));

        byte[] ilBytes = methodBody.GetILAsByteArray();
        if (ilBytes.IsNullOrEmpty())
            return new ArgumentException("Method Body's IL Bytes were null or empty", nameof(method));
        
        throw new NotImplementedException();
    }
}



public class ILMethod
{
    public static ILMethod From(MethodBase method)
    {
        Throw.IfNull(method);

        var methodBody = method.GetMethodBody();
        if (methodBody is null)
            throw new ArgumentException("Could not retrieve method body", nameof(method));

        List<LocalVariableInfo> locals;
        locals = [..methodBody.LocalVariables];
        
        var methodParams = method.GetParameters();
        List<ParameterInfo> parameters;
        if (method.IsStatic)
        {
            parameters = [..methodParams];
        }
        else
        {
            parameters = [new ThisParameter(method), ..methodParams];
        }
        
        
        return new ILMethod
        {
            Module = method.Module,
            DeclaringType = method.DeclaringType,
            GenericArguments = method.GetGenericArguments(),
            Name = method.Name,
            Locals = locals,
            Parameters = parameters,
        };
    }
    
    public Module? Module { get; init; }
    public Type? DeclaringType { get; init; }
    public Type[]? GenericArguments { get; init; }
    public string? Name { get; init; }
    
    public List<LocalVariableInfo> Locals { get; init; }
    public List<ParameterInfo> Parameters { get; init; }
}