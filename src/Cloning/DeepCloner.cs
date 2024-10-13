using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Runtime.Emission;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Cloning;

public static class DeepCloner
{
    private static readonly ConcurrentTypeMap<Delegate> _deepCloneCache;
    private static readonly ConcurrentTypeMap<Delegate> _objectDeepCloneCache;

    private static readonly MethodInfo _deepCloneEmptyMethod;

    static DeepCloner()
    {
        _deepCloneCache = new()
        {
            [typeof(DBNull)] = DbNullDeepClone,
            [typeof(object)] = ObjectDeepClone,
            [typeof(bool)] = UnmanagedDeepClone<bool>,
            [typeof(char)] = UnmanagedDeepClone<char>,
            [typeof(sbyte)] = UnmanagedDeepClone<sbyte>,
            [typeof(byte)] = UnmanagedDeepClone<byte>,
            [typeof(short)] = UnmanagedDeepClone<short>,
            [typeof(ushort)] = UnmanagedDeepClone<ushort>,
            [typeof(int)] = UnmanagedDeepClone<int>,
            [typeof(uint)] = UnmanagedDeepClone<uint>,
            [typeof(long)] = UnmanagedDeepClone<long>,
            [typeof(ulong)] = UnmanagedDeepClone<ulong>,
            [typeof(float)] = UnmanagedDeepClone<float>,
            [typeof(double)] = UnmanagedDeepClone<double>,
            [typeof(decimal)] = UnmanagedDeepClone<decimal>,
            [typeof(DateTime)] = UnmanagedDeepClone<DateTime>,
            [typeof(string)] = StringDeepClone,
        };
        _objectDeepCloneCache = new();

        _deepCloneEmptyMethod = Mirror.Search(typeof(DeepCloner)).TryFindMember(b => b.Public.Static.IsMethod.IsGeneric().Name(nameof(DeepClone))).OkOrThrow();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DBNull DbNullDeepClone(ref readonly DBNull _) => DBNull.Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string StringDeepClone(ref readonly string str) => str;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TUnmanaged UnmanagedDeepClone<TUnmanaged>(ref readonly TUnmanaged unmanaged)
        where TUnmanaged : unmanaged
        => unmanaged;

    private static object ObjectDeepClone(ref readonly object obj)
    {
        var type = obj.GetType();
        if (type == typeof(object)) // prevent infinite recursion
            return obj;

        // Get or create our shim
        var shim = _objectDeepCloneCache.GetOrAdd<DeepClone<object>>(type, static t => CreateDeepCloneShim(t));
        return shim(in obj);
    }
    
    private static DeepClone<object> CreateDeepCloneShim(Type type)
    {
        var deepCloneMethod = _deepCloneEmptyMethod.MakeGenericMethod(type);
        return RuntimeBuilder.EmitDelegate<DeepClone<object>>(emitter => emitter
            .Ldarg(0)
            .Ldind<object>()
            .Unbox(type)
            .Call(deepCloneMethod)
            .Box(type)
            .Ret());
    }
    
    private static DeepClone<T> CreateDeepCloneDelegate<T>(Type type)
    {
        var delegateBuilder = RuntimeBuilder.CreateDelegateBuilder<DeepClone<T>>();
        var emitter = delegateBuilder.Emitter;
        
        // Create a place to store the result
        emitter.DeclareLocal(type, out var clone)
            // and fill it with an uninitialized version
            .Ldtoken(type).Call(EmissionHelper.Type_GetTypeFromHandle_Method)
            .Call(EmissionHelper.GetUninitializedObject_Method)
            .Stloc(clone);
        
        // All instance fields
        var instanceFields = Mirror.Search(type).FindMembers<FieldInfo>(b => b.Instance.IsField).ToList();
        foreach (var field in instanceFields)
        {
            // (ref clone).Field = (in obj).Field
            emitter.Ldloca(clone)
                .Ldarg(0)
                .Ldfld(field)
                .Stfld(field);
        }

        throw new NotImplementedException();
    }
    
    [return: NotNullIfNotNull(nameof(value))]
    public static T? DeepClone<T>(ref readonly T? value)
    {
        if (value is null) return default;
        var del = _deepCloneCache.GetOrAdd<T, DeepClone<T>>(static type => CreateDeepCloneDelegate<T>(type));
        return del(in value);
    }
}