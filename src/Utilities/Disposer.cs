//using ScrubJay.Reflection.Collections;
//
//namespace ScrubJay.Reflection.Utilities;
//
//public static class Disposer
//{
//    /// <summary>
//    /// A delegate that disposes a value.
//    /// </summary>
//    private delegate void StrongDisposeValue<in T>(T? value);
//
//    /// <summary>
//    /// A cache of built <see cref="StrongDisposeValue{T}"/> keyed on the <see cref="Type"/> of value disposed
//    /// </summary>
//    private static readonly DelegateMap _strongDisposeCache = new();
//    
//    /// <summary>
//    /// Creates a <see cref="StrongDisposeValue{T}"/> for the given <paramref name="type"/>
//    /// </summary>
//    private static Delegate? CreateDeepDispose(Type type)
//    {
//        // look for method with the dispose signature
//        var disposeMethod = Reflect(type)
//            .Public.Instance.Methods().Named("Dispose")
//            .Returning(typeof(void))
//            .NoParams
//            .One();
//
//        // all event fields (these can hold references)
//        var eventFields = Reflect(type)
//            .Instance.Events()
//            .SelectWhere(v => v.GetBackingField().NotNull())
//            .ToList();
//
//        // If we have nothing to deal with, we can return null and it will skip execution
//        if (disposeMethod.IsNone() && eventFields.Count == 0)
//        {
//            return null;
//        }
//
//        // Create our dynamic method
//        var runtimeMethod = RuntimeBuilder.BuildDynamicMethod(
//            typeof(StrongDisposeValue<>).MakeGenericType(type),
//            $"{type.Name}_StrongDispose");
//
//        var emitter = runtimeMethod.Emitter
//            .If(disposeMethod,
//                static (emitter, dm) => emitter
//                    .Try((e, end) => e
//                        .Ldarg_0()
//                        .Call(dm)
//                        .Leave(end))
//                    .Swallow<Exception>()
//                    .Finally())
//            .Enumerate(eventFields,
//                // Set the event backing field to null, thus freeing all references
//                static (emitter, field) => emitter
//                    .Ldnull()
//                    .Stfld(field))
//            .Ret();
//
//        // Done with emission
//        return runtimeMethod.TryCreateDelegate().OkOrThrow();
//    }
//
//    /// <summary>
//    ///     Disposes the given <paramref name="thing"/> by calling <see cref="M:IDisposable.Dispose"/> (if it exists)
//    ///     and removing all event handlers.
//    /// </summary>
//    public static void StrongDispose<T>(this T? thing)
//        where T : class
//    {
//        if (thing is null) return;
//        var del = _strongDisposeCache.GetOrAdd<StrongDisposeValue<T>>()
//        if (del is null) return;
//        if (del is StrongDisposeValue<T> strongDispose)
//        {
//            strongDispose(thing);
//        }
//        else
//        {
//            throw new InvalidOperationException();
//        }
//    }
//}