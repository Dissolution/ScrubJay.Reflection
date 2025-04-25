using ScrubJay.Debugging;
using ScrubJay.Reflection.IL.Emission;
using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.Runtime;

public class ArgumentConverter
{
    private static Result<Emit> CanConvertStepOne(Argument source, Argument dest)
    {
#if !(NETFRAMEWORK || NETSTANDARD2_0)
        if (source.Type.IsByRefLike ||
            dest.Type.IsByRefLike)
        {
            Debugger.Break();
            return new NotImplementedException();
        }
#endif

        bool sourceIsByRef = source.IsByRef(out var sourceType);
        bool destIsByRef = dest.IsByRef(out var destType);

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
        
        // ?object -> ?T
        if (sourceType == typeof(object))
        {
            Debugger.Break();
            goto notyet;
        }
        
        // ?T:U -> ?U
        if (CanCast(sourceType, destType).IsOk(out var emit))
        {
            return Ok<Emit>(emitter => emitter
                .Invoke(source.Load)
                .Invoke(emit)
                .Invoke(dest.Store));
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
    
  
    private static Result<Emit> CanConvertField(FieldArgument sourceField, Argument dest)
    {
        if (dest is StackArgument destStack)
        {
            if (destStack.IsByRef(out var destType))
            {
                if (sourceField.Type == destType)
                {
                    // yes, we can
                    return Ok<Emit>(emitter => sourceField.LoadAddr(emitter));
                }

                Debugger.Break();
                return new NotImplementedException();
            }
            else
            {
                if (sourceField.Type == destStack.Type)
                {
                    // yes, we can
                    return Ok<Emit>(emitter => sourceField.Load(emitter));
                }

                Debugger.Break();
                return new NotImplementedException();
            }
        }

        Debugger.Break();
        return new NotImplementedException();
    }

    private static Result<Emit> CanConvertParam(ParameterArgument sourceParam, Argument dest)
    {
        if (dest is FieldArgument destField)
        {
            if (sourceParam.IsByRef(out var sourceType))
            {
                if (dest.IsByRef(out var destType))
                {
                }
                else
                {
                    return Ok<Emit>(emitter => emitter
                        .Ldarg(sourceParam.Parameter.Position)
                        .Ldobj(sourceType)
                        .Invoke(destField.Store));
                }
            }
            else
            {
                if (dest.IsByRef(out var destType))
                {
                }
                else
                {
                }
            }
        }


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

        // Field -> ?
        if (source is FieldArgument sourceField)
        {
            return CanConvertField(sourceField, dest);
        }

        // Parameter -> ?
        if (source is ParameterArgument sourceParam)
        {
            return CanConvertParam(sourceParam, dest);
        }

        Debugger.Break();
        return new NotImplementedException();
    }
}