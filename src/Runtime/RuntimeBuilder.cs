using System.Runtime.Serialization;
using ScrubJay.Reflection.MosDef;
using ScrubJay.Reflection.Naming;

#if NETFRAMEWORK || NETSTANDARD2_0
using Polyfills;
#endif

namespace ScrubJay.Reflection.Runtime;

public static class RuntimeBuilder
{
    public static AssemblyBuilder Assembly { get; }

    public static ModuleBuilder Module { get; }

    static RuntimeBuilder()
    {
        AssemblyName name = new("ScrubJay.Reflection");
        Assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        Module = Assembly.DefineDynamicModule("Runtime");
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

    public static DynamicMethod CreateDynamicMethod(MethodDefinition definition)
    {
        return new DynamicMethod(
            name: CodeHelper.GetValidMemberName(MemberTypes.Method, definition.Name),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: definition._returnType,
            parameterTypes: definition._parameterTypes,
            m: Module,
            skipVisibility: true);
    }

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
        return CreateDynamicMethod(MethodDefinition.Create<D>(name));
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

#endregion
    
    public static HashSet<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(static ass => Result.TryInvoke(ass.GetTypes).OkOr([]))
            .ToHashSet();
    }
}