using ScrubJay.Collections;
using ScrubJay.Reflection.Runtime;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Cloning;

public static class DeepCloner
{
    private static readonly ConcurrentTypeMap<Delegate> _delegateCache;
    private static readonly MethodInfo _deepCloneEmptyMethod;
    
    static DeepCloner()
    {
        _delegateCache = new()
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


        _deepCloneEmptyMethod = Mirror.Search.TryFindMember(typeof(DeepCloner), b => b.Public.Static.Method.Generic.Name(nameof(DeepClone))).OkOrThrow();
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

        var deepCloneMethod = _deepCloneEmptyMethod.MakeGenericMethod(type);
        
    }
    private static DeepClone<object> CreateShim(object obj, Type type, MethodInfo typedDeepCloneMethod)
    {
        //ldarg
        //unbox
    }
    
    
    
    private static Delegate CreateDeepCloneDelegate(Type type)
    {
        
    }

    private static DeepClone<T> CreateDeepCloneDelegate<T>(Type type)
    {
        
    }
    
    [return: NotNullIfNotNull(nameof(value))]
    public static T? DeepClone<T>(this T? value)
    {
        if (value is null) return default;
        var del = _delegateCache.GetOrAdd<T, DeepClone<T>>(static type => CreateDeepCloneDelegate<T>(type));
        return del(in value);
    }
}

[return: NotNullIfNotNull(nameof(value))]
public delegate T? DeepClone<T>(ref readonly T? value);


internal class DeepCloneBuilder
{
    // The ultimate base types
    

    
    public bool DeepClone(ref readonly bool boolean)
    {
        return boolean;
    }
    
    public char DeepClone(ref readonly char character)
    {
        return character;
    }
}

internal class DeepCloneBuilder<TInstance> : DeepCloneBuilder
{





    public DeepClone<TInstance> BuildDelegate()
    {
        var delegateBuilder = RuntimeBuilder.CreateDelegateBuilder<DeepClone<TInstance>>();
        
        // 
        throw new NotImplementedException();
    }
}