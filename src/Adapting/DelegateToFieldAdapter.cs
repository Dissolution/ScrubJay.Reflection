using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public abstract class DelegateToFieldAdapter : DelegateToMemberAdapter,
    IDelegateToMemberAdapter<FieldInfo>
{
    private static Result<D> TryAdaptGetter<D>(FieldInfo field, DelegateInfo delInfo)
        where D : Delegate
    {
        var delParams = delInfo.Parameters;

        Action<Emitter>? emitLoadInstance = null;

        if (field.IsStatic)
        {
            if (delParams.Length > 0)
            {
                if (!delParams[0].HasAttribute<InstanceAttribute>())
                    return GetError(field, delInfo, "Static Field requires Zero Parameters or the first Parameter must be marked with [Instance]");
                if (!AllIncomingSkippable(delParams.AsSpan(1)))
                    return GetError(field, delInfo, "Excess parameters must have default values or be marked with [Params]");
            }
        }
        else
        {
            if (delParams.Length == 0)
                return GetError(field, delInfo, "Instance Field requires an [Instance] parameter");

            var firstParam = delParams[0];

            if (!firstParam.HasAttribute<InstanceAttribute>())
                return GetError(field, delInfo, "Instance Field requires the First Parameter be marked with [Instance]");

            if (!AllIncomingSkippable(delParams.AsSpan(1)))
                return GetError(field, delInfo, "Excess parameters must have default values or be marked with [Params]");

            if (!ArgumentCaster.LoadInstanceFor(firstParam, field)
                .IsOkWithError(out emitLoadInstance, out var loadError))
                return loadError;
        }

        if (!ArgumentCaster.LoadCastStore(field, delInfo.ReturnType)
            .IsOkWithError(out var emitCast, out var error))
            return error;

        var dm = NewDynamicILMethod<D>($"get_{field}");
        dm.Emitter
            .Invoke(emitLoadInstance)
            .Invoke(emitCast)
            .Ret();

        return dm.TryCreateDelegate();
    }

    private static Result<D> TryAdaptSetter<D>(FieldInfo field, DelegateInfo delInfo)
        where D : Delegate
    {
        var delParams = delInfo.Parameters;

        Action<Emitter>? emitLoadInstance = null;
        int offset = 0;
        ParameterArgument valueParam;

        if (field.IsStatic)
        {
            if (delParams.Length == 0)
            {
                return GetError(field, delInfo, "Static Field Setter must have a parameter for the value to store");
            }

            if (delParams.Length == 1)
            {
                if (delParams[0].HasAttribute<InstanceAttribute>())
                    return GetError(field, delInfo, "Static Field Setter must have a parameter for the value to store");

                // this is the value to store
                valueParam = delParams[0];
                offset = 1;
            }
            else if (delParams[0].HasAttribute<InstanceAttribute>())
            {
                // next is the value
                valueParam = delParams[1];
                offset = 2;
            }
            else
            {
                valueParam = delParams[0];
                offset = 1;
            }
        }
        else
        {
            if (delParams.Length < 2)
                return GetError(field, delInfo, "Instance Field Setter must have an [Instance] parameter and the value to store");

            var firstParam = delParams[0];

            if (!firstParam.HasAttribute<InstanceAttribute>())
                return GetError(field, delInfo, "Instance Field requires the First Parameter be marked with [Instance]");

            if (!ArgumentCaster.LoadInstanceFor(firstParam, field)
                .IsOkWithError(out emitLoadInstance, out var loadError))
                return loadError;

            valueParam = delParams[1];
            offset = 2;
        }

        if (!AllIncomingSkippable(delParams.AsSpan(offset)))
            return GetError(field, delInfo, "Excess parameters must have default values or be marked with [Params]");

        if (!ArgumentCaster.LoadCastStore(valueParam, field)
            .IsOkWithError(out var emitCast, out var error))
            return error;

        var dm = NewDynamicILMethod<D>($"set_{field}");
        dm.Emitter
            .Invoke(emitLoadInstance)
            .Invoke(emitCast)
            .If(delInfo.ReturnType != typeof(void),
                emitter =>
        {
            Debug.Assert(delInfo.ReturnType.IsVoidLike());
            // voidlike
            emitter.Ldc_I4_0();
        }).Ret();

        return dm.TryCreateDelegate();
    }

    public static Result<D> TryAdapt<D>(FieldInfo member)
        where D : Delegate
    {
        if (member is null)
            return new ArgumentNullException(nameof(member));

        DelegateInfo delInfo = DelegateInfo.New<D>();

        // if our delegate has a voidlike return, it's a setter
        if (delInfo.ReturnType.IsVoidLike())
        {
            return TryAdaptSetter<D>(member, delInfo);
        }
        
        // otherwise, a getter
        return TryAdaptGetter<D>(member, delInfo);
    }

    private DelegateToFieldAdapter() { }
}