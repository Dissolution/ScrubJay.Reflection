using ScrubJay.Reflection.Runtime.Emission.Fluent;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Runtime.Emission;

public static class EmissionHelper
{
    private static readonly Func<int, Label> _newLabel;

    static EmissionHelper()
    {
        _newLabel = RuntimeBuilder.GenerateDelegate<Func<int, Label>>(generator =>
        {
            generator.Emit(OpCodes.Ldarg_0);
            var ctor = Mirror.Search<Label>.TryFindMember(b => b.Internal.Instance.Constructor.ParameterTypes(typeof(int))).OkOrThrow();
            generator.Emit(OpCodes.Call, ctor);
            generator.Emit(OpCodes.Ret);
        });
    }

    public static Label NewLabel(int index) => _newLabel(index);


    public static IBasicEmitter GetBasicEmitter()
    {
        var instance = new BasicEmitter<IBasicEmitter>();
        var @interface = (IBasicEmitter<IBasicEmitter>)instance;
        var emitter = (IBasicEmitter)@interface;
        return emitter;
    }

    public static IBasicEmitter GetBasicEmitter(ILGenerator ilGenerator)
    {
        var instance = new BackedBasicEmitter<IBasicEmitter>(ilGenerator);
        var @interface = (IBasicEmitter<IBasicEmitter>)instance;
        var emitter = (IBasicEmitter)@interface;
        return emitter;
    }

    public static IFluentEmitter GetFluentEmitter(IBasicEmitter basicEmitter)
    {
        throw new NotImplementedException();
    }
}