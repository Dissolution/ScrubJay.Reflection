using ScrubJay.Reflection.Info;
using ScrubJay.Reflection.Runtime.Naming;
using ScrubJay.Reflection.Searching;
using ConstructorInfo = System.Reflection.ConstructorInfo;

namespace ScrubJay.Reflection.Runtime;

public static class RuntimeBuilder
{
    public static AssemblyBuilder AssemblyBuilder { get; }
    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        var assemblyName = new AssemblyName("ScrubJay.Reflections");
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule("Runtime");
    }


    public static DynamicMethod CreateDynamicMethod(MethodSignature signature)
    {
        return new DynamicMethod(
            name: NameHelper.CreateMemberName(MemberTypes.Method, signature.Name),
            attributes: MethodAttributes.Public | MethodAttributes.Static, // only valid value
            callingConvention: CallingConventions.Standard, // only valid value
            returnType: signature.ReturnType,
            parameterTypes: signature.ParameterTypes,
            m: ModuleBuilder,
            skipVisibility: true);
    }
    
    public static TypeBuilder DefineType(TypeAttributes typeAttributes, string? name = null)
    {
        return ModuleBuilder.DefineType(
            name: NameHelper.CreateMemberName(MemberTypes.TypeInfo, name),
            attr: typeAttributes);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = Mirror.Search<TAttribute>.FindMember(b => b.Instance.Constructor.NoParameters());
        if (ctor is null)
            throw new InvalidOperationException(Dump($"Cannot find an empty {typeof(TAttribute)} constructor."));
        return new CustomAttributeBuilder(ctor, Array.Empty<object>());
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = Searching.MemberSearch.FindConstructor(typeof(TAttribute), ctorArgs);
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException(Dump($"{attributeType} is not an Attribute"));
        var ctor = Searching.MemberSearch.FindConstructor(attributeType, ctorArgs);
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
}


/*

public static DynamicMethod CreateDynamicMethod(DelegateInfo sig, string? name = null)
    {
        return new DynamicMethod(MemberNaming.CreateMemberName(MemberTypes.Method, name),
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            sig.ReturnType,
            sig.ParameterTypes,
            ModuleBuilder,
            true);
    }

    public static RuntimeDelegateBuilder CreateRuntimeDelegateBuilder(DelegateInfo delegateSig, string? name = null)
    {
        var dynamicMethod = CreateDynamicMethod(delegateSig, name);
        return new RuntimeDelegateBuilder(dynamicMethod, delegateSig);
    }

    public static RuntimeDelegateBuilder CreateRuntimeDelegateBuilder(Type delegateType, string? name = null)
        => CreateRuntimeDelegateBuilder(DelegateInfo.For(delegateType), name);
    
    public static RuntimeDelegateBuilder<TDelegate> CreateRuntimeDelegateBuilder<TDelegate>(string? name = null)
        where TDelegate : Delegate
    {
        var dynamicMethod = CreateDynamicMethod(DelegateInfo.For<TDelegate>(), name);
        return new RuntimeDelegateBuilder<TDelegate>(dynamicMethod);
    }

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