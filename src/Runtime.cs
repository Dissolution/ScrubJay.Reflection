namespace ScrubJay.Reflection;

[PublicAPI]
public static class Runtime
{
    public static AssemblyBuilder Assembly { get; } =
        AssemblyBuilder.DefineDynamicAssembly(new("ScrubJay.Reflection.Runtime_Assembly"), AssemblyBuilderAccess.Run);

    public static ModuleBuilder Module { get; } = Assembly.DefineDynamicModule("Runtime_Module");


    private static string CreateMethodName(Type? returnType, Type[]? parameterTypes)
    {
        string name = new TextBuilder()
            .IfAppend(returnType.IsNullOrVoid(), "action", "func")
            .Append('(')
            .EnumerateAndDelimit(parameterTypes, static (tb, type) => tb.Render(type), ", ")
            .If(!returnType.IsNullOrVoid(),
                tb => tb.Append(", ").Render(returnType))
            .Append(')')
            .ToStringAndDispose();

        return Naming.FixIdentifier(name);
    }


    public static DynamicMethod CreateDynamicMethod(
        string? name = null,
        Type? returnType = null,
        Type[]? parameterTypes = null
    )
    {
        return new DynamicMethod(
            name ?? CreateMethodName(returnType, parameterTypes),
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            returnType,
            parameterTypes,
            Module,
            true);
    }
}