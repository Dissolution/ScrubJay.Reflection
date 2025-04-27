using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public sealed class FieldGetterAdapter<I, T> : MemberDelegateAdapter<FieldGetterAdapter<I, T>, FieldInfo, Getter<I, T>>
{
    public override Result<Getter<I, T>> TryAdapt(FieldInfo? field)
    {
        if (field is null)
            return GetEx(field);
        
        bool isStatic = field.IsStatic();
        
        var dm = RuntimeBuilder.BuildDynamicMethod<Getter<I, T>>($"get_{field.NameOf()}");
        var emitter = dm.Emitter;
        if (!isStatic)
        {
            emitter.LoadAsInstance(dm.Parameters[0]);
        }

        // Can we convert the field's type to the output type?
        var canConvert = ArgumentConverter.CanConvert(field, typeof(T));
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

//[PublicAPI]
//public sealed class FieldGetterAdapter<I, T> : MemberDelegateAdapter<FieldInfo, Getter<I, T>>
//{
//    public FieldGetterAdapter(FieldInfo member) : base(member, $"get_{member.NameOf()}")
//    {
//        
//    }
//
//    protected override Result<Action<Emitter>> CanAdapt()
//    {
//        if (Member is null)
//            return GetAdaptError(Member);
//
//        Emissions emissions = [];
//
//        // load instance if we have to
//        if (!TryLoadInstance().IsOkWithError(out var loadEmit, out var error))
//            return error;
//        emissions.Add(loadEmit.Emit);
//
//        // Can we convert the field's type to the output type?
//        var canConvert = ArgumentConverter.CanConvert(Member.FieldType, typeof(T));
//        if (!canConvert.IsOkWithError(out var convertEmit, out error))
//            return error;
//        emissions.Add(convertEmit);
//
//        emissions.Add(emitter => emitter.Ret());
//
//        return Ok(emissions.Combine());
//    }
//}