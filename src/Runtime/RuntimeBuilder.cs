using ScrubJay.Reflection.IL.Emission;
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
        var invoke = DelegateHelper.GetInvokeMethod<D>();
        return CreateDynamicMethod(name, invoke.ReturnType, invoke.GetParameterTypes());
    }

    public static DynamicMethod CreateDynamicMethod(Type delegateType, string? name = null)
    {
        MemberAssert.IsDelegateType(delegateType);
        var invoke = DelegateHelper.GetInvokeMethod(delegateType).SomeOrThrow();
        return CreateDynamicMethod(name, invoke.ReturnType, invoke.GetParameterTypes());
    }

    public static DynamicMethod CreateDynamicMethod(MethodInfo methodSignature, string? name = null)
    {
        Throw.IfNull(methodSignature);
        return CreateDynamicMethod(name, methodSignature.ReturnType, methodSignature.GetParameterTypes());
    }

    public static DynamicMethod CreateDynamicMethod(DelegateInfo info)
    {
        Throw.IfNull(info);
        var dm = CreateDynamicMethod(info.Name, info.ReturnType, info.ParameterTypes);
        dm.DefineParameter(0, info.ReturnParameter.Attributes, info.ReturnParameter.Name);
        var infoParams = info.Parameters;
        foreach (var ip in infoParams)
        {
            dm.DefineParameter(ip.Position + 1, ip.Attributes, ip.Name);
        }

        return dm;
    }

#endregion

    public static DynamicILMethod CreateDynamicILMethod(Type delegateType, string? name = null)
    {
        MemberAssert.IsDelegateType(delegateType);
        return new DynamicILMethod(delegateType, name);
    }

    public static DynamicILMethod CreateDynamicILMethod(DelegateInfo delegateInfo, string? name = null)
    {
        return new DynamicILMethod(delegateInfo, name);
    }

    public static DynamicILMethod<D> CreateDynamicILMethod<D>(string? name = null)
        where D : Delegate
    {
        return new DynamicILMethod<D>(name);
    }


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
        var dm = CreateDynamicILMethod<D>();
        dm.Emitter.Invoke(emit);
        return dm.TryCreateDelegate();
    }

#endregion

#region CustomAttributeBuilder

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = Shard<TAttribute>()
            .Constructors()
            .Instance()
            .OneOrThrow();
        return new CustomAttributeBuilder(ctor, []);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object?[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = Shard<TAttribute>()
            .Instance()
            .Constructors()
            .Accepting(ctorArgs)
            .OneOrThrow();
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType} is not an Attribute");
        var ctor = Shard(attributeType)
            .Instance()
            .Constructors()
            .Accepting(ctorArgs)
            .OneOrThrow();
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

#endregion
}