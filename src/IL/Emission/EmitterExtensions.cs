
using ScrubJay.Reflection.IL.Emission.Arguments;

namespace ScrubJay.Reflection.IL.Emission;

public static class EmitterExtensions
{
    /*
    public static E EmitCast<E>(this E emitter, Arg source, Arg dest)
    {
        source.TryLoadAs(emitter, dest).ThrowIfFailed();
        return emitter;
    }

    public static IFluentILEmitter EmitLoadParameter(this IFluentILEmitter emitter, Arg sourceParameter, Arg destType)
    {
        sourceParameter.TryLoadAs(emitter, destType).ThrowIfFailed();
        return emitter;
    }

   
    
    public static IFluentILEmitter EmitParamsLengthCheck(this IFluentILEmitter emitter,
        ParameterInfo paramsParameter, int length)
    {
        return emitter
            .Ldarg(paramsParameter)
            .Ldlen()
            .Ldc_I4(length)
            .Beq(out var lenEqual)
            .Ldstr($"{length} parameters are required in the params array")
            .Ldstr(paramsParameter.Name)
            .Newobj(Searching.MemberSearch.FindConstructor<ArgumentException>(typeof(string), typeof(string)))
            .Throw()
            .MarkLabel(lenEqual);
    }
    */

    public static E PushValue<E, T>(this E emitter, T? value)
        where E : IOperationEmitter<E>
    {
        return value switch
        {
            null => emitter.Ldnull(),
            bool boolean => boolean ? emitter.Ldc_I4_1() : emitter.Ldc_I4_0(),
            sbyte i8 => emitter.Ldc_I4_S(i8),
            byte u8 => emitter.Ldc_I4(u8),
            short i16 => emitter.Ldc_I4(i16),
            ushort u16 => emitter.Ldc_I4(u16),
            int i32 => emitter.Ldc_I4(i32),
            uint u32 => emitter.Ldc_I8(u32),
            long i64 => emitter.Ldc_I8(i64),
            ulong u64 => emitter.Ldc_I8((long)u64).Conv_U8(),
            float f32 => emitter.Ldc_R4(f32),
            double f64 => emitter.Ldc_R8(f64),
            string str => emitter.Ldstr(str),
            Type type => emitter.Ldtoken(type).Call(EmissionHelper.Type_GetTypeFromHandle_Method),
            MethodInfo method => emitter.Ldtoken(method).Call(EmissionHelper.Method_GetMethodFromHandle_Method),
            ILLocal local => emitter.Ldloc(local),
            _ => throw new NotImplementedException(),
        };
    }
    
    public static E PushDefault<E>(this E emitter, Type type)
        where E : IOperationEmitter<E>, IGenEmitter<E>
    {
        if (type.IsValueType)
        {
            // we have to use a local
            return emitter
                .DeclareLocal(type, out var temp)
                .Ldloca(temp)
                .Initobj(type)
                .Ldloc(temp);
        }
        else
        {
            // defalt is null
            return emitter.Ldnull();
        }
    }
    
    public static Emitter EmitLoadAsInstance(this Emitter emitter, Argument instance)
    {
        return instance.LoadAsInstance(emitter);
    }
    

    public static Emitter EmitLoadParams(this Emitter emitter,
        ParameterInfo paramsParameter,
        ReadOnlySpan<ParameterInfo> destParameters)
    {
        int len = destParameters.Length;
        // None to load?
        if (len == 0)
            return emitter;

        // Params -> Params?
        if (len == 1 && destParameters[0].IsParams())
        {
            emitter.Ldarg(paramsParameter);
        }
        else
        {
            // extract each parameter in turn
            for (var i = 0; i < len; i++)
            {
                emitter.Ldarg(paramsParameter)
                    .Ldc_I4(i)
                    .Ldelem(destParameters[i].ParameterType);
            }
        }

        // Everything will be loaded!
        return emitter;
    }


    public static E EmitThrowException<E, X>(this E emitter, params object?[] exceptionArgs)
        where E : ISimpleEmitter<E>
        where X : Exception
    {
        var exCtor = Reflect<X>()
            .Instance
            .Constructors()
            .Accepting(exceptionArgs)
            .OneOrThrow();
//            
//        
//        
//        var exArgsTypes = exceptionArgs.Select(arg => arg?.GetType()).ToList();
//
//        // Find the ctor we can call with these args
//        var validCtors = typeof(E, X)
//            .GetConstructors(Reflect.Flags.Instance)
//            .Where(ctor =>
//            {
//                var ctorParams = ctor.GetParameters();
//                int ctorParamsCount = ctorParams.Length;
//                if (ctorParamsCount != exArgsTypes.Count) return false;
//                var impl = true;
//                for (var i = 0; i < ctorParamsCount; i++)
//                {
//                    var argType = exArgsTypes[i]?.GetType();
//                    var paramType = ctorParams[i].ParameterType;
//                    if (!argType.Implements(paramType)) return false;
//                }
//                return true;
//            })
//            .ToList();
//        Debugger.Break();
//        var ctor = validCtors[0];
//        foreach (var arg in exceptionArgs)
//        {
//            emitter.LoadValue(arg);
//        }
//        emitter.Newobj(ctor).Throw();
//
//        var il = emitter.ToString();
        Debugger.Break();
        
        return emitter;
    }
}