using ScrubJay.Reflection.IL.Emission;
using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public static class ArgumentCaster
{
    [Flags]
    private enum SafetyLevel
    {
        Unsafe = 0,
        ParameterChecks = 1 << 0,
        ObjectCastChecks = 1 << 1,
        CannotPop = 1 << 2,
        
        Safe = ParameterChecks | ObjectCastChecks | CannotPop,
    }
    
    private static Exception GetError(Argument source, Argument dest, string? info = null)
    {
        string message = TextBuilder.New
            .Append("Cannot transition from ")
            .Render(source)
            .Append(" to ")
            .Render(dest)
            .IfNotNull(info, static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
        return new InvalidOperationException(message)
        {
            Data =
            {
                { "Source", source },
                { "Dest", dest },
            },
        };
    }
    
    private static Result<Emit> Cast(Type sourceType, Type destType)
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

    public static Result<Emit> LoadInstanceFor(Argument source, MemberInfo member)
    {
        if (member.IsStatic())
        {
            return Ok<Emit>(_ => { });
        }
        
        StackArgument destArg;
        Type instanceType = member.DeclaringType.ThrowIfNull();
        if (instanceType.IsValueType)
        {
            instanceType = instanceType.MakeByRefType();
        }
        return LoadCastStore(source, new StackArgument(instanceType));
    }
    
    public static Result<Emit> LoadCastStore(Argument source, Argument dest) //, SafetyLevel safetyLevel = SafetyLevel.Unsafe)
    {
        /* Notes:
         * ?T is shorthand for a Type, `T` that may or may not be a reference
         */
        
        if (source.Type.IsPointer || dest.Type.IsPointer
#if !(NETFRAMEWORK || NETSTANDARD2_0)
            || source.Type.IsByRefLike || dest.Type.IsByRefLike
#endif
        )
        {
            Debugger.Break();
            return new NotImplementedException("Cannot yet handle Pointer or ByRefLike Types");
        }


        bool sourceIsByRef = source.IsByRef(out var sourceType);
        bool destIsByRef = dest.IsByRef(out var destType);

        // Most common transformation
        // ?T -> ?T
        if (sourceType == destType)
        {
            // ref T -> ref T
            if (sourceIsByRef == destIsByRef)
            {
                // load and store
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Invoke(dest.Store));
            }
            
            // ref T -> T
            if (sourceIsByRef)
            {
                // we have to load the value out of the source address before storing
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Ldobj(sourceType)
                    .Invoke(dest.Store));
            }
            
            // T -> ref T
            Debug.Assert(destIsByRef);
            // load address and store
            return Ok<Emit>(emitter => emitter
                .Invoke(source.LoadAddr)
                .Invoke(dest.Store));
        }

        // object check here prevents false positives with T:U
        // ?T -> ?object
        if (destType == typeof(object))
        {
            if (destIsByRef)
                return GetError(source, dest, "ref object destination is not supported");

            // ref T -> object
            if (sourceIsByRef)
            {
                // load the value out of the source reference, then turn it into an object
                return Ok<Emit>(emitter => emitter
                    .Invoke(source.Load)
                    .Ldobj(sourceType)
                    .BoxIfNeeded(sourceType)
                    .Invoke(dest.Store));
            }
            
            // T -> object
            return Ok<Emit>(emitter => emitter
                .Invoke(source.Load)
                .BoxIfNeeded(sourceType)
                .Invoke(dest.Store));
        }

        // danger!: this is the only 'unsafe' source cast that is currently supported
        // ?object -> ?T
        if (sourceType == typeof(object))
        {
            if (sourceIsByRef)
                return GetError(source, dest, "ref object source is not supported");
            

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
        if (Cast(sourceType, destType).IsOk(out var emit))
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

            return Ok<Emit>(_ => { });
        }

        notyet:
        Debug.Write($"{source} -> {dest}");
        Debugger.Break();
        return new NotImplementedException();
    }
    
    public static Result<Emit> LoadParamsCastStore(
        ParameterInfo paramsParameter,
        ReadOnlySpan<Argument> destArgs)
    {
        if (paramsParameter is null)
            return new ArgumentNullException(nameof(paramsParameter));
        if (!paramsParameter.IsParams())
            return new ArgumentException(null, nameof(paramsParameter));

        var paramsArrayElementType = paramsParameter
            .ParameterType
            .GetElementType()
            .ThrowIfNull();
        
        int destArgCount = destArgs.Length;
        
        // None to load?
        if (destArgCount == 0)
            return Ok<Emit>(_ => { });

        Emissions emissions = [];
        
        // each destArg in turn
        for (var i = 0; i < destArgCount; i++)
        {
            if (!LoadParamsToArg(i, destArgs[i])
                .IsOkWithError(out var emit, out var error))
                return error;
            emissions.Add(emit);
        }

        // Everything will be loaded!
        return Ok<Emit>(emissions.Combine());

        Result<Emit> LoadParamsToArg(int index, Argument destArg)
        {
            if (!LoadCastStore(paramsArrayElementType, destArg)
                .IsOkWithError(out var emitLCS, out var error))
                return error;

            return Ok<Emit>(emitter => emitter
                .Ldarg(paramsParameter)
                .PushValue(index)
                .Ldelem(paramsArrayElementType)
                .Invoke(emitLCS));
        }
    }

}