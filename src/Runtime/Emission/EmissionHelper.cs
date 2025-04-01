using System.Runtime.Serialization;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Runtime.Emission;

public static class EmissionHelper
{
    private static readonly Func<int, Label> _newLabel;

    public static MethodInfo TypeGetTypeFromHandleMethod { get; }
    public static MethodInfo GetUninitializedObjectMethod { get; }

    static EmissionHelper()
    {
        // Do not use Emitter in here, it relies on us
        _newLabel = RuntimeBuilder.GenerateDelegate<Func<int, Label>>(generator =>
        {
            generator.Emit(OpCodes.Ldarg_0);
            var ctor = Mirror.For<Label>()
                .Constructors
                .Internal.Instance
                .ParameterTypes<int>()
                .First();
            generator.Emit(OpCodes.Call, ctor);
            generator.Emit(OpCodes.Ret);
        });



        TypeGetTypeFromHandleMethod = Mirror.For<Type>()
            .Methods
            .Public.Static
            .Named(nameof(Type.GetTypeFromHandle))
            .First();

#if NET481 || NETSTANDARD2_0
        GetUninitializedObjectMethod = Mirror.For(typeof(FormatterServices))
            .Methods
            .Named(nameof(FormatterServices.GetUninitializedObject))
            .First();
#else
        GetUninitializedObjectMethod = Mirror.For(typeof(RuntimeHelpers))
            .Methods
            .Named(nameof(RuntimeHelpers.GetUninitializedObject))
            .First();
#endif
    }

    public static Label NewLabel(int index) => _newLabel(index);
}
