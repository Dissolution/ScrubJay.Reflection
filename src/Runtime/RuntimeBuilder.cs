
using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Utilities;
using ScrubJay.Reflection.Validation;

#if NETFRAMEWORK || NETSTANDARD2_0
using Polyfills;
using System.Runtime.Serialization;
#endif

namespace ScrubJay.Reflection.Runtime;

public static class RuntimeBuilder
{
    public static AssemblyBuilder Assembly { get; }

    public static ModuleBuilder Module { get; }

    static RuntimeBuilder()
    {
        AssemblyName name = new("ScrubJay.Reflection.Runtime");
        Assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        Module = Assembly.DefineDynamicModule("RuntimeModule");
    }

    /// <summary>
    /// Returns an uninitialized instance of the given <see cref="Type"/>
    /// </summary>
    public static object GetUninitialized(Type type)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        return FormatterServices.GetUninitializedObject(type);
#else
        return RuntimeHelpers.GetUninitializedObject(type);
#endif
    }

#region DynamicMethod

    public static DynamicMethod CreateDynamicMethod(string? name, Type? returnType, params Type[]? parameterTypes)
    {
        return new DynamicMethod(
            name: CodeHelper.GetValidMemberName(MemberTypes.Method, name),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: returnType,
            parameterTypes: parameterTypes,
            m: Module,
            skipVisibility: true);
    }

    public static DynamicMethod CreateDynamicMethod<D>(string? name = null)
        where D : Delegate
    {
        var invoke = DelegateHelper.InvokeMethod<D>();
        return CreateDynamicMethod(name, invoke.ReturnType, invoke.GetParameterTypes());
    }

    public static DynamicMethod CreateDynamicMethod(Type delegateType, string? name = null)
    {
        TypeAssert.IsDelegate(delegateType);
        var invoke = DelegateHelper.InvokeMethod(delegateType).SomeOrThrow();
        return CreateDynamicMethod(name, invoke.ReturnType, invoke.GetParameterTypes());
    }
    
    public static DynamicMethod CreateDynamicMethod(MethodInfo methodSignature, string? name = null)
    {
        Throw.IfNull(methodSignature);
        return CreateDynamicMethod(name, methodSignature.ReturnType, methodSignature.GetParameterTypes());
    }
    
#endregion

#region Create Delegate

    public static Result<D> TryGenerateDelegate<D>(Action<ILGenerator> generate)
        where D : Delegate
    {
        try
        {
            var dm = CreateDynamicMethod<D>(null);
            var generator = dm.GetILGenerator();
            generate(generator);
            return dm.CreateDelegate<D>();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<D> TryEmitDelegate<D>(Action<Emitter> emit)
        where D : Delegate
    {
        try
        {
            var dm = CreateDynamicMethod<D>(null);
            var generator = dm.GetILGenerator();
            var emitter = new Emitter(generator);
            emit(emitter);
            return dm.CreateDelegate<D>();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

#endregion
    
    #region CustomAttributeBuilder
    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = Reflect<TAttribute>().Constructors.Instance.NoParams.OneOrThrow();
        return new CustomAttributeBuilder(ctor, []);
    }
    
    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object?[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = Reflect<TAttribute>()
            .Instance.Constructors
            .Arguments(ctorArgs)
            .OneOrThrow($"Could not find a {MemberNames.NameOf(typeof(TAttribute))} constructor with that would accept {string.Join(", ", ctorArgs)}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
    
    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType} is not an Attribute");
        var ctor = Reflect(attributeType)
            .Instance.Constructors
            .Arguments(ctorArgs)
            .OneOrThrow($"Could not find a {MemberNames.NameOf(attributeType)} constructor with that would accept {string.Join(", ", ctorArgs)}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
    #endregion
    
}