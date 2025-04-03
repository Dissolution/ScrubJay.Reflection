using System.Diagnostics;
using System.Reflection;
using ScrubJay.Functional;
using ScrubJay.Reflection;
using ScrubJay.Reflection.Collections;
using ScrubJay.Reflection.Extensions;
using ScrubJay.Text;
using ScrubJay.Utilities;

//var alltypes = AppDomain.CurrentDomain
//    .GetAssemblies()
//    .SelectMany(ass => Result.TryInvoke(() => ass.DefinedTypes).OkOr([]))
//    .ToHashSet();

List<Type> types =
[
    typeof(InternalStaticClass),
    typeof(PublicStaticClass),
];
types.AddRange(typeof(Helper).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic));


foreach (var type in types)
{
    Visibility viz = type.Visibility();
    Console.WriteLine($"{type.NameOf()} - {viz}");
}


return 0;

internal static class InternalStaticClass;
public static class PublicStaticClass;

public static class Helper
{
    private static class NestedPrivateStaticClass;
    protected static class NestedProtectedStaticClass;
    internal static class NestedInternalStaticClass;
    public static class NestedPublicStaticClass;
}