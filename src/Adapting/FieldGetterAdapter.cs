using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Adapting;

public class FieldGetterAdapter<I, T> : MemberDelegateAdapter<FieldGetterAdapter<I, T>, FieldInfo, Getter<I, T>>
{
    public override Result<Getter<I, T>> TryAdapt(FieldInfo field)
    {
        Type instanceType = typeof(I);

        var dm = RuntimeBuilder.BuildDynamicMethod<Getter<I, T>>($"get_{field.NameOf()}");
        var emitter = dm.Emitter;

        bool isStatic = IsAssertStatic(field, instanceType);
        Argument fieldArg = field;

        if (!isStatic)
        {
            emitter.LoadAsInstance(dm.Parameters[0]);
        }

        // Can we convert the field's type to the output type?
        var canConvert = ArgumentConverter.CanConvert(fieldArg, typeof(T));
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