using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Runtime;

public abstract class FieldThings : Things
{
    public static Getter<I, T> CreateGetter<I, T>(FieldInfo field)
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
            throw new InvalidOperationException();
        }

        emit(emitter);
        emitter.Ret();

        if (dm.TryCreateDelegate().IsOk(out var del))
        {
            return del;
        }

        var display = dm.ToString();
        Debugger.Break();
        throw new InvalidOperationException();
    }

    public static Setter<I, T> CreateSetter<I, T>(FieldInfo field)
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
            throw new InvalidOperationException();
        }

        emit(emitter);
        emitter.Ret();

        var display = dm.ToString();
        Debugger.Break();

        if (dm.TryCreateDelegate().IsOk(out var del))
        {
            return del;
        }


        throw new InvalidOperationException();
    }
}