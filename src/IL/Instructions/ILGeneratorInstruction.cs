using ScrubJay.Reflection.IL.LabelOffSetManagement;
using static ScrubJay.Reflection.IL.Instructions.ILGeneratorMethod;

namespace ScrubJay.Reflection.IL.Instructions;

public enum ILGeneratorMethod
{
    BeginCatchBlock,
    BeginExceptFilterBlock,
    BeginExceptionBlock,
    EndExceptionBlock,
    BeginFaultBlock,
    BeginFinallyBlock,
    ThrowException,
    UsingNamespace,
    BeginScope,
    EndScope,
    CallManaged,
    CallUnmanaged,
    CallVarargs,
    DeclareLocal,
    DefineLabel,
    MarkLabel,
    WriteLine,
}

public static class ILGeneratorMethodExtensions
{
    public static bool HasArgs(this ILGeneratorMethod method)
    {
        return method is (BeginCatchBlock or 
            BeginExceptionBlock or
            ThrowException or 
            UsingNamespace or 
            >= CallManaged);
    }
}


public class ILGeneratorInstruction : Instruction
{
    public ILGeneratorMethod ILGenMethod { get;  }

    public override sealed int Size => 0;
    
    public ILGeneratorInstruction(ILGeneratorMethod ilGenMethod)
    {
        ILGenMethod = ilGenMethod;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Align(ILGenMethod.AsString().AsSpan(), 22, alignment: Alignment.Right)
            .IfAppend(!ILGenMethod.HasArgs(), "()");
    }
}

public sealed class ILGeneratorMarkLabelInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public ILGeneratorMarkLabelInstruction(ILLabel label)
        : base(MarkLabel)
    {
        Label = label;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append('(')
            .Render(Label)
            .Append(')');
    }
}

public sealed class ILGeneratorDefineLabelInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public ILGeneratorDefineLabelInstruction(ILLabel label)
        : base(DefineLabel)
    {
        Label = label;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append('(')
            .Render(Label)
            .Append(')');
    }
}

public sealed class ILGeneratorBeginExceptionBlockInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public ILGeneratorBeginExceptionBlockInstruction(ILLabel label)
        : base(BeginExceptionBlock)
    {
        Label = label;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append('(')
            .Render(Label)
            .Append(')');
    }
}

public sealed class ILGeneratorDeclareLocalInstruction : ILGeneratorInstruction
{
    public ILLocal Local { get; }

    public ILGeneratorDeclareLocalInstruction(ILLocal local)
        : base(DeclareLocal)
    {
        Local = local;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append('(')
            .Render(Local)
            .Append(')');
    }
}

public sealed class ILGeneratorCallVarargsInstruction : ILGeneratorInstruction
{
    public OpCode OpCode { get; }
    public MethodInfo Method { get; }
    public Type[]? OptionalParameterTypes { get; }

    public ILGeneratorCallVarargsInstruction(OpCode opCode, MethodInfo method, Type[]? optionalParameterTypes = null) 
        : base(CallVarargs)
    {
        OpCode = opCode;
        Method = method;
        OptionalParameterTypes = optionalParameterTypes;        
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"({OpCode:@}, {Method:@}, {OptionalParameterTypes:@})");
    }
}

public sealed class ILGeneratorCallUnmanagedInstruction : ILGeneratorInstruction
{
    public CallingConvention CallingConvention { get; }
    public Type? ReturnType { get; }
    public Type[]? ParameterTypes { get; }

    public ILGeneratorCallUnmanagedInstruction(CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes) 
        : base(CallUnmanaged)
    {
        CallingConvention = callingConvention;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"({CallingConvention:@}, {ReturnType:@}, {ParameterTypes:@})");
    }
}

public sealed class ILGeneratorCallManagedInstruction : ILGeneratorInstruction
{
    public CallingConventions CallingConventions { get; }
    public Type? ReturnType { get; }
    public Type[]? ParameterTypes { get; }
    public Type[]? OptionalParameterTypes { get; }

    public ILGeneratorCallManagedInstruction(CallingConventions callingConventions, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes) 
        : base(CallManaged)
    {
        CallingConventions = callingConventions;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
        OptionalParameterTypes = optionalParameterTypes;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"({CallingConventions:@}, {ReturnType:@}, {ParameterTypes:@}, {OptionalParameterTypes:@})");
    }
}
    



public sealed class ILGeneratorBeginCatchBlockInstruction : ILGeneratorInstruction
{
    public Type? ExceptionType { get; }
    
    public ILGeneratorBeginCatchBlockInstruction(Type? exceptionType) : base(BeginCatchBlock)
    {
        this.ExceptionType = exceptionType;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"({ExceptionType:@})");
    }
}

public sealed class ILGeneratorThrowExceptionInstruction : ILGeneratorInstruction
{
    public Type ExceptionType { get; }
    
    public ILGeneratorThrowExceptionInstruction(Type exceptionType) : base(ThrowException)
    {
        this.ExceptionType = exceptionType;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"({ExceptionType:@})");
    }
}

public sealed class ILGeneratorWriteLineInstruction : ILGeneratorInstruction
{
    public string Text { get; }
    
    public ILGeneratorWriteLineInstruction(string text) : base(WriteLine)
    {
        this.Text = text;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"(\"{Text}\")");
    }
}

public sealed class ILGeneratorUsingNamespaceInstruction : ILGeneratorInstruction
{
    public string Namespace { get; }
    
    public ILGeneratorUsingNamespaceInstruction(string @namespace) : base(UsingNamespace)
    {
        this.Namespace = @namespace;
    }

    public override void RenderTo(TextBuilder builder)
    {
        builder.Invoke(base.RenderTo!)
            .Append($"(\"{Namespace}\")");
    }
}