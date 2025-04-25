using ScrubJay.Debugging;
using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.IL.Emission.Arguments;

public class ArgumentConverter
{
    private static Result<Emit> CanConvertStepOne(Argument source, Argument dest)
    {
        if (source.Type.IsPointer
            || dest.Type.IsPointer
#if !(NETFRAMEWORK || NETSTANDARD2_0)
            || source.Type.IsByRefLike
            || dest.Type.IsByRefLike
#endif
        )
        {
            Debugger.Break();
            return new NotImplementedException("Cannot yet handle Pointer or ByRefLike Types");
        }


        bool sourceIsByRef = source.IsByRef(out var sourceType);
        bool destIsByRef = dest.IsByRef(out var destType);

        // Fastest + most common is just type to same type
        // ?T -> ?T
        if (sourceType == destType)
        {
            if (sourceIsByRef == destIsByRef)
            {
                // ez mode
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Invoke(dest.Store));
            }
            else
            {
                if (sourceIsByRef)
                {
                    // ref T -> T
                    return Ok<Emit>(emitter => emitter
                        .Invoke(source.Load)
                        .Ldobj(sourceType)
                        .Invoke(dest.Store));
                }
                else
                {
                    Debug.Assert(destIsByRef);
                    // T -> ref T
                    return Ok<Emit>(emitter => emitter
                        .Invoke(source.LoadAddr)
                        .Invoke(dest.Store));
                }
            }
        }

        // object check here prevents false positives with T:U
        // ?T -> ?object
        if (destType == typeof(object))
        {
            if (destIsByRef)
                goto notyet;

            // ref T -> object
            if (sourceIsByRef)
            {
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Ldobj(sourceType)
                    .BoxIfNeeded(sourceType)
                    .Invoke(dest.Store));
            }
            // T -> object
            else
            {
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .BoxIfNeeded(sourceType)
                    .Invoke(dest.Store));
            }
        }

        // object check here cleans up the unsafe source cast
        // ?object -> ?T
        if (sourceType == typeof(object))
        {
            if (sourceIsByRef)
                goto notyet;

            // danger!
            // todo: emit arg check?

            if (destType.IsValueType)
            {
                if (destIsByRef)
                {
                    return Ok<Emit>(emitter => emitter
                        .Invoke(source.Load)
                        .Unbox(destType)
                        .Invoke(dest.Store));
                }
                else
                {
                    return Ok<Emit>(emitter => emitter
                        .Invoke(source.Load)
                        .Unbox_Any(destType)
                        .Invoke(dest.Store));
                }
            }
            else
            {
                if (destIsByRef)
                    goto notyet;

                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Castclass(destType)
                    .Invoke(dest.Store));
            }
        }

        // class -> class|interface
        // value -> interface
        // ?T:U -> ?U
        if (CanCast(sourceType, destType).IsOk(out var emit))
        {
            return Ok<Emit>(emitter => emitter
                .Invoke(source.Load)
                .Invoke(emit)
                .Invoke(dest.Store));
        }

        // ?T -> ?void
        if (destType == typeof(void))
        {
            // Nothing can hold void except the stack
            if (dest is not StackArgument)
                return new InvalidOperationException("Only the stack can hold void");

            // pop if on stack, otherwise do nothing
            if (source is StackArgument)
            {
                return Ok<Emit>(emitter => emitter.Pop());
            }

            return Ok<Emit>(emitter => emitter);
        }

        notyet:
        Debug.Write($"{source} -> {dest}");
        Debugger.Break();
        return new NotImplementedException();
    }

    private static Result<Emit> CanCast(Type sourceType, Type destType)
    {
        Debug.Assert(sourceType is not null);
        Debug.Assert(sourceType != typeof(object));
        Debug.Assert(sourceType!.IsByRef.Not());
        Debug.Assert(destType is not null);
        Debug.Assert(destType!.IsByRef.Not());
        Debug.Assert(destType != typeof(object));
        Debug.Assert(destType != sourceType);

        if (sourceType.IsValueType)
        {
            if (destType.IsValueType)
            {
                // no?
                Debugger.Break();
                return new NotSupportedException();
            }
            else if (destType.IsInterface)
            {
                Type[] interfaceTypes = sourceType.GetInterfaces();
                if (Sequence.Contains(interfaceTypes, destType))
                {
                    // value -> box -> cast
                    return Ok<Emit>(emitter => emitter
                        .Box(sourceType)
                        .Castclass(destType));
                }

                // Cannot
                Debugger.Break();
                return new InvalidOperationException($"{sourceType.NameOf()} does not inherit {destType.NameOf()}");
            }
            else
            {
                Debug.Assert(destType.IsClass);
                // no?
                Debugger.Break();
                return new NotSupportedException();
            }
        }
        else if (sourceType.IsInterface)
        {
            Debugger.Break();
        }
        else
        {
            Debug.Assert(sourceType.IsClass);

            if (destType.IsValueType)
            {
                Debugger.Break();
            }
            else if (destType.IsInterface)
            {
                Type[] interfaceTypes = sourceType.GetInterfaces();
                if (Sequence.Contains(interfaceTypes, destType))
                {
                    // castclass
                    return Ok<Emit>(emitter => emitter.Castclass(destType));
                }

                // Cannot
                Debugger.Break();
                return new InvalidOperationException($"{sourceType.NameOf()} does not inherit {destType.NameOf()}");
            }
            else
            {
                Debug.Assert(destType.IsClass);

                // check for subclass
                Type? subClass = sourceType.BaseType;
                while (subClass is not null)
                {
                    if (subClass == destType)
                    {
                        // castclass
                        return Ok<Emit>(emitter => emitter.Castclass(destType));
                    }
                    subClass = subClass.BaseType;
                }

                // Cannot
                Debugger.Break();
                return new InvalidOperationException($"{sourceType.NameOf()} does not inherit {destType.NameOf()}");
            }
        }


        // Cannot
        Debugger.Break();
        return new NotImplementedException();
    }


    public static Result<Emit> CanConvert(Argument source, Argument dest)
    {
        var result = CanConvertStepOne(source, dest);
        if (result.IsOkWithError(out var emit, out var error))
        {
            return emit;
        }
        else
        {
            Debug.WriteLine(error.Dump());
            Debugger.Break();
        }

        return new NotImplementedException();
    }
}