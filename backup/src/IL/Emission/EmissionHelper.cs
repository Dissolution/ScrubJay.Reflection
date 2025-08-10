using ScrubJay.Reflection.Searching;
#if NETFRAMEWORK || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

namespace ScrubJay.Reflection.IL.Emission;

[PublicAPI]
public static class EmissionHelper
{
    public static MethodInfo Type_GetTypeFromHandle_Method { get; }
    public static MethodInfo Method_GetMethodFromHandle_Method { get; }
    public static MethodInfo GetUninitializedObject_Method { get; }
    public static MethodInfo Delegate_GetInvocationList_Method { get; }
    
    static EmissionHelper()
    {
        Type_GetTypeFromHandle_Method = Shard<Type>()
            .Public().Static().Methods()
            .Named(nameof(Type.GetTypeFromHandle))
            .OneOrThrow();

        Method_GetMethodFromHandle_Method = Shard<MethodBase>()
            .Public().Static().Methods()
            .Named(nameof(MethodBase.GetMethodFromHandle))
            .WithParameters(1)
            .OneOrThrow();

        Delegate_GetInvocationList_Method = Shard<Delegate>()
            .Instance().Methods()
            .Named(nameof(Delegate.GetInvocationList))
            .NoParameters()
            .Returning<Delegate[]>()
            .OneOrThrow();
        
        
#if NETFRAMEWORK || NETSTANDARD2_0
        GetUninitializedObject_Method = Reflect(typeof(FormatterServices))
            .Public().Static().Methods()
            .Named(nameof(FormatterServices.GetUninitializedObject))
            .OneOrThrow();
#else
        GetUninitializedObject_Method = Mirror
            .Member<MethodInfo>(() => RuntimeHelpers.GetUninitializedObject(default!))
            .OkOrThrow();
        
        /*
        GetUninitializedObject_Method = Reflect(typeof(RuntimeHelpers))
            .Public().Static().Methods
            .Named(nameof(RuntimeHelpers.GetUninitializedObject))
            .OneOrThrow();
        */
#endif
    }
    
    public static string? GetLocalName(string? name)
    {
        var buffer = name.AsSpan();
       
        // trim the name
        buffer = buffer.Trim();
        
        // if empty, no valid name
        if (buffer.Length == 0)
            return null;
        
        // check for spaces
        var spaceIndex = buffer.LastIndexOf(' ');
        if (spaceIndex >= 0)
        {
            // if found, trim to after the space
            buffer = buffer.Slice(spaceIndex + 1);
        }

        return buffer.AsString();
    }
    
    public static string? GetLabelName(string? name)
    {
        throw new NotImplementedException();
    }
}