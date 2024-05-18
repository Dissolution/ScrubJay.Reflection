using ScrubJay.Reflection.Info;
using ScrubJay.Reflection.Runtime.Emission;
using ScrubJay.Reflection.Runtime.Emission.Emitters;
using ScrubJay.Reflection.Runtime.Naming;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Runtime;

public static class RuntimeBuilder
{
    public static AssemblyBuilder AssemblyBuilder { get; }
    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        var assemblyName = new AssemblyName("ScrubJay.Reflections");
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule(nameof(Runtime));
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
        var ctor = Mirror.Search<TAttribute>()
            .TryFindMember(b => b.Instance.Constructor.NoParameters())
            .OkOrThrow();
        return new CustomAttributeBuilder(ctor, Array.Empty<object>());
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = Mirror.Search<TAttribute>()
            .TryFindMember(b => b.Instance.Constructor.ParameterTypes(ctorArgs))
            .OkOrThrow();
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType} is not an Attribute");
        var ctor = Mirror.Search(attributeType)
            .TryFindMember(b => b.Instance.Constructor.ParameterTypes(ctorArgs))
            .OkOrThrow();
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
    
    public static DynamicMethod CreateDynamicMethod(DelegateSignature signature)
    {
        var returnSig = signature.ReturnSignature;
        var paramsSigs = signature.ParameterSignatures;
        
        DynamicMethod dynamicMethod = new DynamicMethod(
            name: NameHelper.CreateMemberName(MemberTypes.Method, signature.Name),
            attributes: MethodAttributes.Public | MethodAttributes.Static, // only valid value
            callingConvention: CallingConventions.Standard, // only valid value
            returnType: returnSig.Type,
            parameterTypes: signature.GetParameterTypes(),
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
    
    public static TDelegate GenerateDelegate<TDelegate>(Action<ILGenerator> generateDelegate)
        where TDelegate : Delegate
    {
        var builder = CreateDelegateBuilder<TDelegate>();
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