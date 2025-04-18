using ScrubJay.Reflection.Runtime;

namespace ScrubJay.Reflection.Utilities;

public static class Disposer
{
    /// <summary>
    /// A delegate that disposes a value.
    /// </summary>
    private delegate void StrongDisposeValue<in T>(T value);

    /// <summary>
    /// A cache of built <see cref="StrongDisposeValue{T}"/> keyed on the <see cref="Type"/> of value disposed
    /// </summary>
    private static readonly ConcurrentTypeMap<Delegate?> _strongDisposeCache = new();


    /// <summary>
    ///     Creates a <see cref="StrongDisposeValue{T}"/> for the given <paramref name="type"/>.
    /// </summary>
    private static Delegate? CreateDeepDispose(Type type)
    {
        // Dispose method?
        var disposeMethod = Reflect(type)
            .Public.Instance.Methods.Named("Dispose")
            .Returning(typeof(void))
            .NoParams
            .One();
        
        // all event fields
        var eventFields = Reflect(type)
            .Instance.Events
            .Select(v => v.GetBackingField())
            .WhereNotNull()
            .ToList();

        // If we have nothing to deal with, we can return null and it will skip execution
        if (disposeMethod.IsNone() && eventFields.Count == 0)
        {
            return null;
        }

        // Create our dynamic method
        var runtimeMethod = new DynamicMethodBuilder(
            typeof(StrongDisposeValue<>).MakeGenericType(type),
            $"{type.Name}_StrongDispose");
        var emitter = runtimeMethod.Emitter;
        
        // Do we have a dispose method?
        if (disposeMethod.IsSome(out var dm))
        {
            emitter.Try((e, end) => e
                    .Ldarg_0()
                    .Call(dm)
                    .Br(end))
                .Catch<Exception>((e, end) => e.Pop())
                .Finally();
//            
//            // try { value.Dispose(); }
//            emitter.BeginExceptionBlock(out var lblEndTry)
//                .Ldarg_0()
//                .Call(dm)
//                .Br(lblEndTry)
//                // catch (Exception ex) { // ignore }
//                .BeginCatchBlock<Exception>()
//                .Pop()
//                .EndExceptionBlock()
//                .MarkLabel(lblEndTry);
        }

        // Event Fields?
        foreach (var field in eventFields)
        {
            // Set the event backing field to null, thus freeing all references
            emitter.Ldarg_0()
                .Ldnull()
                .Stfld(field);
        }

        // Fin
        emitter.Ret();

        // Done with emission
        return runtimeMethod.TryCreateDelegate().OkOrThrow();
    }

    /// <summary>
    ///     Disposes the given <paramref name="thing"/> by calling <see cref="M:IDisposable.Dispose"/> (if it exists)
    ///     and removing all event handlers.
    /// </summary>
    public static void DeepDispose<T>(this T? thing)
        where T : class
    {
        if (thing is null) return;
        var del = _strongDisposeCache.GetOrAdd<T>(CreateDeepDispose);
        if (del is null) return;
        if (del is StrongDisposeValue<T> strongDispose)
        {
            strongDispose(thing);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}