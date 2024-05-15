using System.Linq.Expressions;

namespace ScrubJay.Reflection.Info;

public sealed record class DelegateSignature
{
    public static DelegateSignature For(MethodBase method)
    {
        return new DelegateSignature
        {
            Name = method.Name,
            ParameterSignatures = method.GetParameterSignatures(),
            ReturnSignature = method.GetReturnSignature(),
        };
    }

    public static DelegateSignature For<TDelegate>(TDelegate? _ = default)
        where TDelegate : Delegate
    {
        var invokeMethod = Delegates.GetInvokeMethod<TDelegate>();
        return new DelegateSignature
        {
            _delegateType = typeof(TDelegate),
            Name = typeof(TDelegate).Name,
            ParameterSignatures = invokeMethod.GetParameterSignatures(),
            ReturnSignature = invokeMethod.GetReturnSignature(),
        };
    }

    public static DelegateSignature For(Type delegateType)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Not a valid Delegate Type", nameof(delegateType));
        var invokeMethod = Delegates.GetInvokeMethod(delegateType)!;
        return new DelegateSignature
        {
            _delegateType = delegateType,
            Name = delegateType.Name,
            ParameterSignatures = invokeMethod.GetParameterSignatures(),
            ReturnSignature = invokeMethod.GetReturnSignature(),
        };
    }
    
    
    private Type? _delegateType;

    public required string Name { get; init; } = "Invoke";
    
    public required IReadOnlyList<ParameterSignature> ParameterSignatures { get; init; } = Array.Empty<ParameterSignature>();

    public required ReturnSignature ReturnSignature { get; init; } = ReturnSignature.Void;

    private Type CreateDelegateType()
    {
        var parameterTypes = this.ParameterSignatures;
        Type[] typeArgs = new Type[parameterTypes.Count + 1];
        for (var i = 0; i < parameterTypes.Count; i++)
        {
            typeArgs[i] = parameterTypes[i].Type;
        }
        typeArgs[^1] = this.ReturnSignature.Type;
        return Expression.GetDelegateType(typeArgs);
    }
    
    
    public Type GetOrCreateDelegateType()
    {
        return _delegateType ??= CreateDelegateType();
    }

    public Type[] GetParameterTypes()
    {
        var paramSigs = this.ParameterSignatures;
        var types = new Type[paramSigs.Count];
        for (var i = 0; i < paramSigs.Count; i++)
        {
            types[i] = paramSigs[i].Type;
        }
        return types;
    }
}