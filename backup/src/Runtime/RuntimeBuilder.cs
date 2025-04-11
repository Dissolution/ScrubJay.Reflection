using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Runtime.Emission;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Signatures;

namespace ScrubJay.Reflection.Runtime;

public static class RuntimeBuilder
{
    private readonly static TypeBuilder _runtimeTypeBuilder;

    public static AssemblyBuilder AssemblyBuilder { get; }
    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        var assemblyName = new AssemblyName("ScrubJay.Reflections");
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule("Runtime");
        _runtimeTypeBuilder = ModuleBuilder.DefineType(
            "Runtime",
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class);
    }


    public static TypeBuilder DefineType(TypeAttributes typeAttributes, string? name = null)
    {
        return ModuleBuilder.DefineType(
            name: NameHelper.MemberName(name, MemberTypes.TypeInfo),
            attr: typeAttributes);
    }

    public static MethodBuilder DefineMethod(MethodSig sig)
    {
        return _runtimeTypeBuilder.DefineMethod(
            name: NameHelper.MemberName(sig.Name, MemberTypes.Method),
            attributes: sig.MethodAttributes,
            callingConvention: sig.Access.HasFlags(Access.Static) ? CallingConventions.Standard : CallingConventions.HasThis,
            returnType: sig.Return.CompositeType,
            parameterTypes: sig.Parameters.CompositeTypes());
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = Mirror.For<TAttribute>().Constructors.Instance.NoParameters.First();
        return new CustomAttributeBuilder(ctor, []);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = Mirror.For<TAttribute>()
            .Constructors
            .Instance
            .Parameters(ctorArgs)
            .FirstOrDefault();
        if (ctor is null)
            throw new ArgumentException($"Could not find a {MemberNames.NameOf(typeof(TAttribute))} constructor with that would accept {string.Join(", ", ctorArgs)}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType} is not an Attribute");
        var ctor = Mirror.For(attributeType)
            .Constructors
            .Instance
            .Parameters(ctorArgs)
            .FirstOrDefault();
        if (ctor is null)
            throw new ArgumentException($"Could not find a {MemberNames.NameOf(attributeType)} constructor with that would accept {string.Join(", ", ctorArgs)}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }


    
    public static DelegateBuilder<TDelegate> CreateDelegateBuilder<TDelegate>(string? name)
        where TDelegate : Delegate
    {
        var invokeMethod = DelegateHelper.GetInvokeMethod<TDelegate>();
        var dm = CreateDynamicMethod(name, invokeMethod.ReturnType, invokeMethod.GetParameterTypes());
        return new DelegateBuilder<TDelegate>(dm);
    }


    public static TDelegate EmitDelegate<TDelegate>(Action<FluentILEmitter> emitDelegate)
       where TDelegate : Delegate
    {
        var dm = CreateDynamicMethod<TDelegate>(null);
        var generator = dm.GetILGenerator();
        var emitter = new FluentILEmitter(generator);
        emitDelegate(emitter);
        return dm.CreateDelegate<TDelegate>();
    }
    
    public static TDelegate BuildDelegate<TDelegate>(Action<DelegateBuilder<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        var dm = CreateDynamicMethod<TDelegate>(null);
        var builder = new DelegateBuilder<TDelegate>(dm);
        buildDelegate(builder);
        return builder.TryBuild().OkOrThrow();
    }
}
/*
public static DynamicMethod CreateDynamicMethod(DelegateSignature signature)
{
    var returnSig = signature.ReturnSignature;
    var paramsSigs = signature.ParameterSignatures;

    DynamicMethod dynamicMethod = new DynamicMethod(
        name: NameHelper.CreateMemberName(MemberTypes.Method, signature.Name),
        attributes: MethodAttributes.Public | MethodAttributes.Static, // only valid value
        callingConvention: CallingConventions.Standard, // only valid value
        returnType: returnSig.Type,
        parameterTypes: signature.GetOrCreateParameterTypes(),
        m: ModuleBuilder,
        skipVisibility: true);

    dynamicMethod.SetReturnSignature(returnSig);
    for (var i = 0; i < paramsSigs.Count; i++)
    {
        dynamicMethod.SetParameterSignature(i, paramsSigs[i]);
    }

    return dynamicMethod;
}

public static DelegateBuilder CreateDelegateBuilder(DelegateSignature signature)
    => new DelegateBuilder(signature);

public static DelegateBuilder<TDelegate> CreateDelegateBuilder<TDelegate>(string? name = null)
    where TDelegate : Delegate
    => new DelegateBuilder<TDelegate>(name);

public static Delegate BuildDelegate(DelegateSignature signature, Action<DelegateBuilder> buildDelegate)
{
    var builder = CreateDelegateBuilder(signature);
    buildDelegate(builder);
    return builder.CreateDelegate();
}

public static TDelegate BuildDelegate<TDelegate>(Action<DelegateBuilder<TDelegate>> buildDelegate)
    where TDelegate : Delegate
{
    var builder = CreateDelegateBuilder<TDelegate>();
    buildDelegate(builder);
    return builder.CreateDelegate();
}

public static TDelegate BuildDelegate<TDelegate>(string? name, Action<DelegateBuilder<TDelegate>> buildDelegate)
    where TDelegate : Delegate
{
    var builder = CreateDelegateBuilder<TDelegate>(name);
    buildDelegate(builder);
    return builder.CreateDelegate();
}

public static Delegate GenerateDelegate(DelegateSignature signature, Action<ILGenerator> generateDelegate)
{
    var builder = CreateDelegateBuilder(signature);
    generateDelegate(builder.ILGenerator);
    return builder.CreateDelegate();
}



public static TDelegate GenerateDelegate<TDelegate>(string? name, Action<ILGenerator> generateDelegate)
    where TDelegate : Delegate
{
    var builder = CreateDelegateBuilder<TDelegate>(name);
    generateDelegate(builder.ILGenerator);
    return builder.CreateDelegate();
}

public static Delegate EmitDelegate(DelegateSignature signature, Action<ICleanEmitter> emitDelegate)
{
    var builder = CreateDelegateBuilder(signature);
    emitDelegate(builder.Emitter);
    return builder.CreateDelegate();
}

public static TDelegate EmitDelegate<TDelegate>(Action<ICleanEmitter> emitDelegate)
    where TDelegate : Delegate
{
    var builder = CreateDelegateBuilder<TDelegate>();
    emitDelegate(builder.Emitter);
    return builder.CreateDelegate();
}

public static TDelegate EmitDelegate<TDelegate>(string? name, Action<ICleanEmitter> emitDelegate)
    where TDelegate : Delegate
{
    var builder = CreateDelegateBuilder<TDelegate>(name);
    emitDelegate(builder.Emitter);
    return builder.CreateDelegate();
}
}
*/

/*


    public static Delegate CreateDelegate(DelegateInfo delegateSig, string? name, Action<RuntimeDelegateBuilder> buildDelegate)
    {
        var runtimeDelegateBuilder = CreateRuntimeDelegateBuilder(delegateSig, name);
        buildDelegate(runtimeDelegateBuilder);
        return runtimeDelegateBuilder.CreateDelegate();
    }

    public static Delegate CreateDelegate(Type delegateType, string? name, Action<RuntimeDelegateBuilder> buildDelegate)
        => CreateDelegate(DelegateInfo.For(delegateType), name, buildDelegate);

    public static TDelegate CreateDelegate<TDelegate>(string? name, Action<RuntimeDelegateBuilder<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        var runtimeDelegateBuilder = CreateRuntimeDelegateBuilder<TDelegate>(name);
        buildDelegate(runtimeDelegateBuilder);
        return runtimeDelegateBuilder.CreateDelegate();
    }

    public static Delegate CreateDelegate(DelegateInfo delegateSig, Action<IFluentILEmitter> emitDelegate)
    {
        return CreateDelegate(delegateSig, null, emitDelegate);
    }

    public static Delegate CreateDelegate(Type delegateType, Action<IFluentILEmitter> emitDelegate)
    {
        return CreateDelegate(delegateType, null, emitDelegate);
    }

    public static Delegate CreateDelegate(DelegateInfo delegateSig, string? name, Action<IFluentILEmitter> emitDelegate)
    {
        var runtimeMethod = CreateRuntimeDelegateBuilder(delegateSig, name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }

    public static Delegate CreateDelegate(Type delegateType, string? name, Action<IFluentILEmitter> emitDelegate)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Must be a delegate", nameof(delegateType));
        var runtimeMethod = CreateRuntimeDelegateBuilder(delegateType, name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }


    public static TDelegate CreateDelegate<TDelegate>(Action<IFluentILEmitter> emitDelegate)
        where TDelegate : Delegate
    {
        return CreateDelegate<TDelegate>(null, emitDelegate);
    }

    public static TDelegate CreateDelegate<TDelegate>(string? name, Action<IFluentILEmitter> emitDelegate)
        where TDelegate : Delegate
    {
        var runtimeMethod = CreateRuntimeDelegateBuilder<TDelegate>(name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }


}

*/