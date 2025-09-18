namespace ScrubJay.Reflection;

[PublicAPI]
public static class Runtime
{
    public static class Builder
    {
        public static AssemblyBuilder Assembly { get; } =
            AssemblyBuilder.DefineDynamicAssembly(new("ScrubJay.Reflection.Runtime_Assembly"),
                AssemblyBuilderAccess.Run);

        public static ModuleBuilder Module { get; } = Assembly.DefineDynamicModule("Runtime_Module");


        private static string CreateMethodName(Type? returnType, Type[]? parameterTypes)
        {
            string name = new TextBuilder()
                .If(returnType.IsNullOrVoid(), "action", "func")
                .Append('(')
                .Delimit(", ", parameterTypes)
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

#if NET5_0_OR_GREATER
    public static IReadOnlySet<Type> AllTypes { get; }
#else
    public static IReadOnlyCollection<Type> AllTypes { get; }
#endif

    static Runtime()
    {
        HashSet<Type> allTypes = new();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    allTypes.Add(type);
                }
            }
            catch
            {
                // Assemblies are finicky, ignore all exceptions
            }
        }

        AllTypes = allTypes;
    }
}