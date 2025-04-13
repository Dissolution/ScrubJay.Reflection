using ScrubJay.Reflection.Searching;
#if NETFRAMEWORK || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public static class EmissionHelper
{
    public static MethodInfo Type_GetTypeFromHandle_Method { get; }
    public static MethodInfo GetUninitializedObject_Method { get; }

    static EmissionHelper()
    {
        Type_GetTypeFromHandle_Method = Reflect<Type>()
            .Public.Static.Methods
            .Named(nameof(Type.GetTypeFromHandle))
            .OneOrThrow();

#if NETFRAMEWORK || NETSTANDARD2_0
        GetUninitializedObject_Method = Reflect(typeof(FormatterServices))
            .Public.Static.Methods
            .Named(nameof(FormatterServices.GetUninitializedObject))
            .OneOrThrow();
#else
        GetUninitializedObject_Method = Mirror
            .Member<MethodInfo>(() => RuntimeHelpers.GetUninitializedObject(default!))
            .OkOrThrow();
        
        /*
        GetUninitializedObject_Method = Reflect(typeof(RuntimeHelpers))
            .Public.Static.Methods
            .Named(nameof(RuntimeHelpers.GetUninitializedObject))
            .OneOrThrow();
        */
#endif
    }
}