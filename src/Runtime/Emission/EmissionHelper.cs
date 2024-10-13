#if NET481 || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Runtime.Emission;

public static class EmissionHelper
{
    private static readonly Func<int, Label> _newLabel;

    public static MethodInfo Type_GetTypeFromHandle_Method { get; }
    public static MethodInfo GetUninitializedObject_Method { get; }

    static EmissionHelper()
    {
        _newLabel = RuntimeBuilder.GenerateDelegate<Func<int, Label>>(generator =>
        {
            generator.Emit(OpCodes.Ldarg_0);
            var ctor = Mirror.Search<Label>()
                .TryFindMember(b => b.Internal.Instance.IsConstructor.Parameters<int>())
                .OkOrThrow();
            generator.Emit(OpCodes.Call, ctor);
            generator.Emit(OpCodes.Ret);
        });

        Type_GetTypeFromHandle_Method = Mirror
            .TryFindMember<MethodInfo>((RuntimeTypeHandle handle) => Type.GetTypeFromHandle(handle))
            .OkOrThrow();

#if NET481 || NETSTANDARD2_0
        GetUninitializedObject_Method = Mirror
            .TryFindMember<MethodInfo>((Type type) => FormatterServices.GetUninitializedObject(type))
            .OkOrThrow();
#else
        
        GetUninitializedObject_Method = Mirror
            .TryFindMember<MethodInfo>((Type type) => RuntimeHelpers.GetUninitializedObject(type))
            .OkOrThrow();
#endif
    }

    public static Label NewLabel(int index) => _newLabel(index);
}