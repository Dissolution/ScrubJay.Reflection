using ScrubJay.Reflection.IL.Emission;
using static InlineIL.IL;

namespace ScrubJay.Reflection.Cloning;

[PublicAPI]
public static class Cloner
{
    [return: NotNullIfNotNull(nameof(value))]
    internal delegate T? DeepCloneValue<T>(T? value);

    private static readonly ConcurrentTypeMap<Delegate> _deepCloneDelegateCache = [];

    static Cloner()
    {
        TryAdd<string>(DeepCloneString);
    }

    internal static bool TryAdd<T>(DeepCloneValue<T> deepClone)
    {
        return _deepCloneDelegateCache.TryAdd<T>(deepClone);
    }
    

    [return: NotNullIfNotNull(nameof(str))]
    private static string? DeepCloneString(string? str) => str;

    private static readonly MethodInfo _deepCloneUnmanagedMethod = Shard(typeof(Cloner))
        .Private()
        .Static()
        .Methods()
        .Named(nameof(DeepCloneUnmanaged))
        .AreGeneric(1)
        .OneOrThrow();

    private static readonly MethodInfo _deepCloneMethod = Shard(typeof(Cloner))
        .Public().Static().Methods().Named(nameof(DeepClone))
        .AreGeneric(1)
        .OneOrThrow();
    
    private static U DeepCloneUnmanaged<U>(U value)
        where U : unmanaged
    {
        Emit.Ldarga(nameof(value));
        Emit.Ldobj<U>();
        return Return<U>();
    }

    private static Delegate DeepCloneArray(Type arrayType)
    {
        Debug.Assert(arrayType.IsArray);
        var elementType = arrayType.GetElementType();
        Debug.Assert(elementType is not null);
        
        var deepCloneItemMethod = _deepCloneMethod.MakeGenericMethod(elementType);

        var methodBuilder =RuntimeBuilder.CreateDynamicILMethod(
            typeof(DeepCloneValue<>).MakeGenericType(arrayType));
        methodBuilder.Emitter
        // load the array's length and store it
            .DeclareLocal<int>(out var arrayLen)
            .Ldarg_0()
            .Ldlen()
            .Stloc(arrayLen)
            // Create the cloned array local
            .DeclareLocal(arrayType, out var clone)
            // use the length to create an empty array and store it in clone
            .Ldloc(arrayLen)
            .Newarr(elementType!)
            .Stloc(clone)
            // int i = 0;
            .DeclareLocal<int>(out var i)
            .Ldc_I4_0()
            .Stloc(i)
            // define do and while labels, then branch to the while check
            .DefineLabel(out var lblDo)
            .DefineLabel(out var lblWhile)
            .Br(lblWhile)
            .MarkLabel(lblDo)
            // inside do
            // load the clone array and the index in the array we'll be storing at (sets up stack for later)
            .Ldloc(clone)
            .Ldloc(i)
            // load the original array, index
            .Ldarg(0)
            .Ldloc(i)
            // load the original's item, copy it, and store in clone
            .Ldelem(elementType!)
            .Call(deepCloneItemMethod)
            .Stelem(elementType!)
            // increment i
            .Ldloc(i)
            .Ldc_I4_1()
            .Add()
            .Stloc(i)
            // while (i < arrayLen)
            .MarkLabel(lblWhile)
            .Ldloc(i)
            .Ldloc(arrayLen)
            .Blt(lblDo)
            // we have finished, load and return the clone
            .Ldloc(clone)
            .Ret();
        
        return methodBuilder.TryCreateDelegate().OkOrThrow();
    }
    
    private static Delegate /*DeepCloneValue<T>*/ CreateDeepCloneDelegate(Type type)
    {
        if (type.IsUnmanaged())
        {
            return Delegate.CreateDelegate(
                typeof(DeepCloneValue<>).MakeGenericType(type),
                _deepCloneUnmanagedMethod.MakeGenericMethod(type));
        }
        else if (type == typeof(string))
        {
            Debugger.Break();
            return DeepCloneString;
        }
        
        Debug.Assert(!type.IsPrimitive);

        if (type.IsArray)
            return DeepCloneArray(type);

        var fields = Shard(type).Instance().Fields().ToList();

        if (fields.Count == 0)
            throw new NotImplementedException();
        
        var methodBuilder = new DynamicILMethod(typeof(DeepCloneValue<>).MakeGenericType(type));
        var emitter = methodBuilder.Emitter;
        
        // declare the clone
        emitter.DeclareLocal(type, out var clone);
        
        // init the clone so it's not null
        if (type.IsValueType)
        {
            emitter.Ldloca(clone)
                .Initobj(type)
                .Nop();
        }
        else
        {
            emitter.Ldtoken(type)
                .Call(EmissionHelper.Type_GetTypeFromHandle_Method)
                .Call(EmissionHelper.GetUninitializedObject_Method)
                .Castclass(type)
                .Stloc(clone)
                .Nop();
        }
        
        
        foreach (var field in fields)
        {
            if (type.IsValueType)
            {
                emitter.Ldloca(clone);
            }
            else
            {
                emitter.Ldloc(clone);
            }
            
            emitter
                .Ldarg(0)
                .Ldfld(field)
                .Call(_deepCloneMethod.MakeGenericMethod(field.FieldType))
                .Stfld(field)
                .Nop();
        }

        emitter.Ldloc(clone)
            .Ret();

        return methodBuilder.TryCreateDelegate().OkOrThrow();
    }


    [return: NotNullIfNotNull(nameof(value))]
    public static T? DeepClone<T>(this T? value)
    {
        if (value is null)
            return default(T)!;
        
        try
        {
            var del = _deepCloneDelegateCache.GetOrAdd<T>(static type => CreateDeepCloneDelegate(type));
            var deepCloner = del as DeepCloneValue<T>;
       
            var clone = deepCloner!(value);
            return clone;
        }
        catch (Exception ex)
        {
            var dump = ex.Render();
            Console.WriteLine(dump);
            Debugger.Break();
            throw;
        }
       
    }
}