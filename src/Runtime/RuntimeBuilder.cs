using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using ScrubJay.Reflection.MosDef;

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


    public static DynamicMethod NewDynamicMethod(MethodDefinition definition)
    {
        return new DynamicMethod(
            name: definition.Name,
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: definition._returnType,
            parameterTypes: definition._parameterTypes,
            m: Module,
            skipVisibility: true);
    }
}