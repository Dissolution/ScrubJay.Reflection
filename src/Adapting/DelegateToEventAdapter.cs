using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.Exceptions;
using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public abstract class DelegateToEventAdapter : DelegateToMemberAdapter
{
    private static Result<D> TryAdaptAdderToField<D>(FieldInfo field, DelegateInfo delInfo)
    {
        return new NotImplementedException();
    }

    private static Result<D> TryAdaptRemoverToField<D>(FieldInfo field, DelegateInfo delInfo)
    {
        return new NotImplementedException();
    }

    private static Result<RaiseHandler<I>> TryAdaptRaiserToField<I>(FieldInfo backingField, DelegateInfo delInfo)
    {
        var eventHandlerType = backingField.FieldType;

        var invokeMethod = eventHandlerType
            .InvokeMethod()
            .SomeOrThrow();

        var dm = NewDynamicILMethod<RaiseHandler<I>>($"raise_{backingField.NameOf()}");

        // The event field is a Delegate with the signature of the event
        // e.g. EventHandler<T> is (object?,EventArgs<T>)
        // We can access its parts!
        var emitter = dm.Emitter
            .If<Emitter>(!backingField.IsStatic,
                e => new ParameterArgument(dm.Parameters[0]).LoadAsInstance(e))
            .Ldfld(backingField)
            .DeclareLocal(eventHandlerType, out var eventHandler)
            .Stloc(eventHandler)
            .Ldloc(eventHandler)
            .Branch(false, out var finished)
            .Ldloc(eventHandler)
            .Call(EmissionHelper.Delegate_GetInvocationList_Method)
            .DeclareLocal<Delegate[]>(out var delegates)
            .Stloc(delegates)
            .Ldloc(delegates)
            .Ldlen()
            .DeclareLocal<int>(out var len)
            .Stloc(len)
            .DeclareLocal<int>(out var i)
            .PushValue(0)
            .Stloc(i)
            .Branch(out var whileCheck)
            .MarkLabel(out var doStart)
            .Ldloc(delegates)
            .Ldloc(i)
            .Ldelem<Delegate>();

        if (!ArgumentCaster.LoadParamsCastStore(
                dm.Parameters[1],
                invokeMethod.GetParameters().ConvertAll(p => new StackArgument(p.ParameterType)))
            .IsOkWithError(out var emitLPCS, out var error)) 
            return error;

        emitter.Invoke(emitLPCS)
            .Call(invokeMethod)
            .If(invokeMethod.ReturnType != typeof(void), e => e.Pop())
            .Ldloc(i)
            .PushValue(1)
            .Add()
            .Stloc(i)
            .MarkLabel(whileCheck)
            .Ldloc(i)
            .Ldloc(len)
            .Blt(doStart)
            .MarkLabel(finished)
            .Ret();


        string il = emitter.ToString()!;
        Debugger.Break();

        return dm.TryCreateDelegate().OkOrThrow();
    }

    public static Result<D> TryAdaptAdd<D>(EventInfo eventInfo)
        where D : Delegate
    {
        if (eventInfo is null)
            return new ArgumentNullException(nameof(eventInfo));

        var delInfo = DelegateInfo.New<D>();

        // find the add method
        var addMethod = eventInfo.AddMethod;
        if (addMethod is not null)
        {
            return DelegateToMethodAdapter.TryAdapt<D>(addMethod);
        }

        // we can try to use the backing field and manually call the add
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            return TryAdaptAdderToField<D>(backingField, delInfo);
        }

        return GetError(eventInfo, delInfo, "Cannot find AddMethod nor backing field");
    }

    public static Result<D> TryAdaptRemove<D>(EventInfo eventInfo)
        where D : Delegate
    {
        if (eventInfo is null)
            return new ArgumentNullException(nameof(eventInfo));

        var delInfo = DelegateInfo.New<D>();

        // find the remove method
        var removeMethod = eventInfo.RemoveMethod;
        if (removeMethod is not null)
        {
            return DelegateToMethodAdapter.TryAdapt<D>(removeMethod);
        }

        // we can try to use the backing field and manually call the remove
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            return TryAdaptRemoverToField<D>(backingField, delInfo);
        }

        return GetError(eventInfo, delInfo, "Cannot find RemoveMethod nor backing field");
    }

    public static Result<RaiseHandler<I>> TryAdapt<I>(EventInfo eventInfo)
    {
        if (eventInfo is null)
            return new ArgumentNullException(nameof(eventInfo));

        var delInfo = DelegateInfo.New<RaiseHandler<I>>();

        // Find Raiser
        var raiseMethod = eventInfo.RaiseMethod;
        if (raiseMethod is not null)
        {
            // We somehow have one!
            return DelegateToMethodAdapter.TryAdapt<RaiseHandler<I>>(raiseMethod);
        }

        // backing field?
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            return TryAdaptRaiserToField<I>(backingField, delInfo);
        }

        throw new ReflectionException($"Cannot find a way to raise {eventInfo}");
    }

    private DelegateToEventAdapter() { }
}