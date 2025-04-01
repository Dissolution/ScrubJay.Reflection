#nullable enable

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ScrubJay.Sigil.Utilities;

internal class TypeOnStack : IEquatable<TypeOnStack>, IComparable<TypeOnStack>
{
    public static bool operator ==(TypeOnStack? a, TypeOnStack? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }
    public static bool operator !=(TypeOnStack? a, TypeOnStack? b)
    {
        if (ReferenceEquals(a, b)) return false;
        if (a is null || b is null) return true;
        return !a.Equals(b);
    }

    private static readonly ConcurrentDictionary<Type, TypeOnStack> _cache = [];
    private static readonly Dictionary<Tuple<CallingConventions, Type, Type, Type[]>, TypeOnStack> _knownFunctionPointerCache = [];


    private static TypeOnStack FromCache(Type type)
    {
        return _cache.GetOrAdd(type, static t => new TypeOnStack(t));
    }

    public static TypeOnStack Get<T>()
    {
        return Get(typeof(T));
    }

    public static TypeOnStack Get(Type type)
    {
        if (type.ContainsGenericParameters)
        {
            throw new InvalidOperationException("Sigil does not currently support generic types; found " + type);
        }

        if (type == typeof(char)) type = typeof(ushort);

        var ret = FromCache(type);

        return
            new TypeOnStack(ret.Type)
            {
                CallingConvention = ret.CallingConvention,
                HasAttachedMethodInfo = ret.HasAttachedMethodInfo,
                InstanceType = ret.InstanceType,
                ParameterTypes = ret.ParameterTypes,
                ReturnType = ret.ReturnType,
                UsedBy = new(),
            };
    }

    public static TypeOnStack GetKnownFunctionPointer(CallingConventions conv, Type instanceType, Type returnType, Type[] parameterTypes)
    {
        var key = Tuple.Create(conv, instanceType, returnType, parameterTypes);

        TypeOnStack ret;

        lock (_knownFunctionPointerCache)
        {
            if (!_knownFunctionPointerCache.TryGetValue(key, out ret))
            {
                ret =
                    new TypeOnStack(typeof(NativeIntType))
                    {
                        HasAttachedMethodInfo = true,
                        CallingConvention = conv,
                        InstanceType = instanceType,
                        ReturnType = returnType,
                        ParameterTypes = parameterTypes,
                        UsedBy = new HashSet<Tuple<InstructionAndTransitions, int>>(),
                    };

                _knownFunctionPointerCache[key] = ret;
            }
        }

        return ret;
    }





    public Type Type { get; private set; }

    public bool IsReference { get { return Type.IsByRef; } }

    public bool IsPointer { get { return Type.IsPointer; } }

    public bool IsArray { get { return Type.IsArray; } }

    public bool IsPointerToValueType { get { return Type.IsPointer && Type.GetElementType().IsValueType; } }

    public bool IsInterface { get { return Type.IsInterface; } }

    public bool HasAttachedMethodInfo { get; private set; }

    public CallingConventions CallingConvention { get; private set; }

    public Type InstanceType { get; private set; }

    public Type ReturnType { get; private set; }

    public Type[] ParameterTypes { get; private set; }

    public bool IsMarkable { get { return UsedBy != null; } }

    public bool IsVoid { get { return Type == typeof(void); } }

    internal HashSet<Tuple<InstructionAndTransitions, int>> UsedBy { get; set; }

    public TypeOnStack(Type type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Call to indicate that something on the stack was used
    /// as the #{index}'d (starting at 0) parameter to the {code}
    /// opcode.
    /// </summary>
    public void Mark(InstructionAndTransitions instr, int index)
    {
        UsedBy.Add(Tuple.Create(instr, index));
    }

    /// <summary>
    /// Returns the # of times this value was used as the given #{index}'d parameter to the {code} instruction.
    /// </summary>
    public int CountMarks(OpCode code, int ix)
    {
        return UsedBy.Count(c => c.Item1.Instruction.Equals(code) && c.Item2 == ix);
    }

    /// <summary>
    /// Returns the total number of times this value was marked.
    /// </summary>
    public int CountMarks()
    {
        return UsedBy.Count;
    }

    public bool IsAssignableFrom(TypeOnStack other)
    {
        return ExtensionMethods.IsAssignableFrom(this, other);
    }

    public int CompareTo(TypeOnStack? other)
    {
        if (other is null) return 1;
        // ugh, really?
        return string.CompareOrdinal(this.ToString(), other.ToString());
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(this, obj)) return 0;
        if (ReferenceEquals(obj, null)) return 1;
        // ugh, really?
        return string.CompareOrdinal(this.ToString(), obj.ToString());
    }

    public bool Equals(TypeOnStack other)
    {
        if (ReferenceEquals(this, other)) return true;

        // There's not exact map of NativeInt in .NET; but once it's on the stack IntPtr can be manipulated similarly
        if (this.Type == typeof(NativeIntType) && other.Type == typeof(IntPtr) || other.Type == typeof(NativeIntType) && this.Type == typeof(IntPtr))
        {
            return this.IsPointer == other.IsPointer && this.IsReference == other.IsReference;
        }

        return
            this.Type == other.Type &&
            this.IsPointer == other.IsPointer &&
            this.IsReference == other.IsReference;
    }

    public override bool Equals(object? obj)
    {
        return obj is TypeOnStack typeOnStack && Equals(typeOnStack);
    }

    public override int GetHashCode()
    {
        return
            (int)(
                Type.GetHashCode() ^
                (IsPointer ? 0x0000FFFF : 0) ^
                (IsReference ? 0xFFFF0000 : 0)
            );
    }


    public override string ToString()
    {
        var ret = Type.FullName;

        if (Type == typeof(NativeIntType)) ret = "native int";
        if (Type == typeof(NullType)) ret = "null";
        if (Type == typeof(int)) ret = "int";
        if (Type == typeof(long)) ret = "long";
        if (Type == typeof(float)) ret = "float";
        if (Type == typeof(double)) ret = "double";
        if (Type == typeof(AnyPointerType)) ret = "pointer";
        if (Type == typeof(AnyByRefType)) ret = "by ref";
        if (Type == typeof(OnlyObjectType)) ret = typeof(object).ToString();

        return ret;
    }
}
