using ScrubJay.Fluent;
using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Signatures;

namespace ScrubJay.Reflection.Runtime;

public class MethodSigBuilder : FluentRecordBuilder<MethodSigBuilder, MethodSig>
{
    public MethodSigBuilder() : base(new()) { }

    public MethodSigBuilder Named(string? name)
    {
        _record.Name = NameHelper.MemberName(name, MemberTypes.Method);
        return _builder;
    }

    public MethodSigBuilder Returns(Action<DynamicParameterBuilder> buildReturn)
    {
        var builder = new DynamicParameterBuilder(_record.Return);
        buildReturn(builder);
        return _builder;
    }

    public MethodSigBuilder Returns(Type returnType)
    {
        _record.Return.RefKind = returnType.RefKind(out var type);
        _record.Return.Type = type;
        return _builder;
    }

    public MethodSigBuilder Returns<T>() => Returns(typeof(T));

    public MethodSigBuilder Parameter(Action<DynamicParameterBuilder> buildParameter)
    {
        ParameterSig sig = new()
        {
            Index = _record.Parameters.Count,
        };
        _record.Parameters.Add(sig);
        var builder = new DynamicParameterBuilder(sig);
        buildParameter(builder);
        return _builder;
    }

    public MethodSigBuilder Parameter(Type argType)
    {
        ParameterSig sig = new()
        {
            Index = _record.Parameters.Count,
            RefKind = argType.RefKind(out var type),
            Type = type,
        };
        _record.Parameters.Add(sig);
        return _builder;
    }
    public MethodSigBuilder Parameter<T>() => Parameter(typeof(T));

    public MethodSigBuilder Parameters(params Action<DynamicParameterBuilder>[] buildParameters)
    {
        foreach (var buildParameter in buildParameters)
        {
            _ = Parameter(buildParameter);
        }
        return _builder;
    }

    public MethodSigBuilder Parameters(params Type[] argsTypes)
    {
        foreach (var argType in argsTypes)
        {
            _ = Parameter(argType);
        }
        return _builder;
    }

    private void DefineParam(DynamicMethod dm, int index, ParameterSig sig)
    {
        //MethodBuilder mb = dm;

        ParameterBuilder? param = dm.DefineParameter(index, sig.ParameterAttributes, sig.Name);

        //ParameterBuilder

        if (sig.Default.IsSome(out var defaultValue))
        {
            param!.SetConstant(defaultValue);
        }

        foreach (AttributeSig? attr in sig.Attributes)
        {
            if (attr is ConstructedAttributeSig caSig)
            {
                var attrBuilder = new CustomAttributeBuilder(caSig.Constructor!, caSig.Arguments);
                param!.SetCustomAttribute(attrBuilder);
            }
        }
    }

    public MethodSigBuilder Like<TDelegate>()
        where TDelegate : Delegate
    {
        var delType = typeof(TDelegate);
        var invokeMethod = DelegateHelper.GetInvokeMethod(delType);
        _record = invokeMethod;
        return _builder;
    }

    public DynamicMethod GetDynamicMethod()
    {
        DynamicMethod dm =  new DynamicMethod(
            name: _record.Name ?? NameHelper.MemberName(null, MemberTypes.Method),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: _record.Return.CompositeType,
            parameterTypes: _record.Parameters.CompositeTypes(),
            m: RuntimeBuilder.ModuleBuilder,
            skipVisibility: true);

        int idx = 0;
        // return
        DefineParam(dm, idx++, _record.Return);
        // args
        foreach (var argParam in _record.Parameters)
        {
            DefineParam(dm, idx++, argParam);
        }

        Debugger.Break();
        return dm;
    }
}
