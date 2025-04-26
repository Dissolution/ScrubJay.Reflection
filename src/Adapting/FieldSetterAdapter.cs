using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Adapting;

public class FieldSetterAdapter<I, T> : MemberDelegateAdapter<FieldSetterAdapter<I, T>, FieldInfo, Setter<I, T>>
{
    public override Result<Setter<I, T>> TryAdapt(FieldInfo field)
    {
        Type instanceType = typeof(I);

        var dm = RuntimeBuilder.BuildDynamicMethod<Setter<I, T>>($"set_{field.NameOf()}");
        var emitter = dm.Emitter;

        bool isStatic = IsAssertStatic(field, instanceType);
        FieldArgument fieldArg = field;
        
        if (!isStatic)
        {
            emitter.LoadAsInstance(dm.Parameters[0]);
        }

        // Can we convert the input T type to the Field's Type?
        var canConvert = ArgumentConverter.CanConvert(
            dm.Parameters[1],
            fieldArg);
        if (!canConvert.IsOkWithError(out var emit, out var error))
        {
            Console.WriteLine(error);
            Debugger.Break();
            return error;
        }

        emit(emitter);
        emitter.Ret();

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