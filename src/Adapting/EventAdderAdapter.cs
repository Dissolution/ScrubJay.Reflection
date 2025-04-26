using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Adapting;

public class EventAdderAdapter<I, H> : MemberDelegateAdapter<EventAdderAdapter<I, H>, EventInfo, AddHandler<I, H>>
    where H : Delegate
{
    public override Result<AddHandler<I, H>> TryAdapt(EventInfo eventInfo)
    {
        // find the add method
        var addMethod = eventInfo.AddMethod;
        if (addMethod is not null)
        {
            return MethodDelegateAdapter<AddHandler<I, H>>.Instance.TryAdapt(addMethod);
        }

        // we can try to use the backing field and manually call the add
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            return new NotImplementedException();
        }

        return GetEx(eventInfo);
    }
}

public class EventRemoverAdapter<I, H> : MemberDelegateAdapter<EventRemoverAdapter<I, H>, EventInfo, RemoveHandler<I, H>>
    where H : Delegate
{
    public override Result<RemoveHandler<I, H>> TryAdapt(EventInfo eventInfo)
    {
        // find the remove method
        var addMethod = eventInfo.RemoveMethod;
        if (addMethod is not null)
        {
            return MethodDelegateAdapter<RemoveHandler<I, H>>.Instance.TryAdapt(addMethod);
        }

        // we can try to use the backing field and manually call the remove
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            return new NotImplementedException();
        }

        return GetEx(eventInfo);
    }
}

public class EventRaiseAdapter<I> : MemberDelegateAdapter<EventRaiseAdapter<I>, EventInfo, RaiseHandler<I>>
{
    public override Result<RaiseHandler<I>> TryAdapt(EventInfo eventInfo)
    {
        // Find Raiser
        var raise = eventInfo.RaiseMethod;
        if (raise is not null)
        {
            // We somehow have one!
            return MethodDelegateAdapter<RaiseHandler<I>>.Instance.TryAdapt(raise);
        }

        // backing field?
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            var eventHandlerType = backingField.FieldType;
            var invokeMethod = eventHandlerType
                .InvokeMethod()
                .SomeOrThrow();

            var dm = RuntimeBuilder.BuildDynamicMethod<RaiseHandler<I>>($"raise_{eventInfo.Name}");

            // The event field is a Delegate with the signature of the event
            // e.g. EventHandler<T> is (object?,EventArgs<T>)
            // We can access its parts!
            var emitter = dm.Emitter
                .If<Emitter>(!backingField.IsStatic,
                    e => e.EmitLoadAsInstance(dm.Parameters[0]))
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
                .Ldelem<Delegate>()
                .EmitLoadParams(dm.Parameters[1], invokeMethod.GetParameters())
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

        throw new ReflectionException($"Cannot find a way to raise {eventInfo}");
    }
}
