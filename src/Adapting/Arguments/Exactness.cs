namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly struct Exactness :
#if NET7_0_OR_GREATER
    IEqualityOperators<Exactness, Exactness, bool>,
    IComparisonOperators<Exactness, Exactness, bool>,
#endif
    IEquatable<Exactness>,
    IComparable<Exactness>
{
    private const uint BASE_CLASS_OFFSET = /*   */ 0000001U;
    private const uint INTERFACE_OFFSET = /*    */ 0000100U;

    private const uint TO_OBJECT_OFFSET = /*    */ 0010000U;
    private const uint POP_OFFSET = /*          */ 0100000U;
    private const uint FROM_OBJECT_OFFSET = /*  */ 1000000U;

    public static bool operator ==(Exactness left, Exactness right) => left.Equals(right);
    public static bool operator !=(Exactness left, Exactness right) => !left.Equals(right);
    public static bool operator >(Exactness left, Exactness right) => left.CompareTo(right) > 0;
    public static bool operator >=(Exactness left, Exactness right) => left.CompareTo(right) >= 0;
    public static bool operator <(Exactness left, Exactness right) => left.CompareTo(right) < 0;
    public static bool operator <=(Exactness left, Exactness right) => left.CompareTo(right) <= 0;


    public static Exactness Exact()
    {
        return new(0U);
    }

    public static Exactness BaseClass(int levels)
    {
        if (levels < 1)
            throw new ArgumentOutOfRangeException(nameof(levels), levels, "There must be at least one level of base class nesting");
        return new(BASE_CLASS_OFFSET * (uint)levels);
    }

    public static Exactness Interface(int levels)
    {
        if (levels < 1)
            throw new ArgumentOutOfRangeException(nameof(levels), levels, "There must be at least one level of base class nesting");
        return new(INTERFACE_OFFSET * (uint)levels);
    }

    public static Exactness ToObject()
    {
        return new Exactness(TO_OBJECT_OFFSET);
    }

    public static Exactness Pop(Type type)
    {
        if (type.IsValueType)
            return new(POP_OFFSET);
        return new(POP_OFFSET * 5);
    }

    public static Exactness FromObject()
    {
        return new(FROM_OBJECT_OFFSET);
    }


    [FieldOffset(0)]
    private readonly uint _exactness;

    private Exactness(uint exactness)
    {
        _exactness = exactness;
    }

    public int CompareTo(Exactness other) => _exactness.CompareTo(other._exactness);

    public bool Equals(Exactness other) => _exactness.Equals(other._exactness);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Exactness exactness)
            return Equals(exactness);
        if (obj is uint u32)
            return u32 == _exactness;
        if (obj is int i32)
            return i32 == _exactness;
        return false;
    }

    public override int GetHashCode() => (int)_exactness;

    public override string ToString() => _exactness.ToString();
}