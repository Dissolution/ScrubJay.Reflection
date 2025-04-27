using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public sealed class FieldSetterAdapter<I, T> : MemberDelegateAdapter<FieldSetterAdapter<I, T>, FieldInfo, Setter<I, T>>
{
    public override Result<Setter<I, T>> TryAdapt(FieldInfo? field)
    {
        if (field is null)
            return GetEx(field);
        
        bool isStatic = field.IsStatic();

        var dm = RuntimeBuilder.BuildDynamicMethod<Setter<I, T>>($"set_{field.NameOf()}");
        var emitter = dm.Emitter;
        if (!isStatic)
        {
            emitter.LoadAsInstance(dm.Parameters[0]);
        }

        // Can we convert the input T type to the Field's Type?
        var canConvert = ArgumentConverter.CanConvert(
            dm.Parameters[1],
            field);
        if (!canConvert.IsOkWithError(out var emit, out var error))
        {
            Console.WriteLine(error);
            Debugger.Break();
            return error;
        }

        emitter.Invoke(emit).Ret();

        if (dm.TryCreateDelegate().IsOkWithError(out var del, out error))
        {
            return del;
        }
        else
        {
            Console.WriteLine(error);
            Debugger.Break();
            return error;
        }
    }
}