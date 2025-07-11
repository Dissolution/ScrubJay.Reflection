namespace ScrubJay.Reflection.IL.LabelOffSetManagement;

/// <summary>
/// A wrapper around the positive part of an <see cref="int"/> where any negative value means the same as <see cref="Unknown"/>
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Auto, Size = 4)]
public readonly struct ILOffset :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILOffset, ILOffset, bool>,
    IComparisonOperators<ILOffset, ILOffset, bool>,
    IAdditionOperators<ILOffset, ILOffset, ILOffset>,
    ISubtractionOperators<ILOffset, ILOffset, ILOffset>,
#endif
    IEquatable<ILOffset>,
    IComparable<ILOffset>,
    IRenderable
{
    public static implicit operator ILOffset(int offset) => new(offset);
    public static explicit operator int(ILOffset offset) => offset._offset;

    public static bool operator ==(ILOffset left, ILOffset right) => left.Equals(right);
    public static bool operator !=(ILOffset left, ILOffset right) => !left.Equals(right);

    public static bool operator >(ILOffset left, ILOffset right) => left.CompareTo(right) > 0;
    public static bool operator >=(ILOffset left, ILOffset right) => left.CompareTo(right) >= 0;
    public static bool operator <(ILOffset left, ILOffset right) => left.CompareTo(right) < 0;
    public static bool operator <=(ILOffset left, ILOffset right) => left.CompareTo(right) <= 0;


    public static ILOffset operator +(ILOffset left, ILOffset right)
    {
        if (left == Unknown || right == Unknown)
            return Unknown;
        long i64 = (long)left._offset + (long)right._offset;
        if (i64 > (long)int.MaxValue)
            return Unknown;
        return new((int)i64);
    }
    
    public static ILOffset operator -(ILOffset left, ILOffset right)
    {
        if (left == Unknown || right == Unknown)
            return Unknown;
        int offset = left._offset - right._offset;
        if (offset < 0)
            return Unknown;
        return new(offset);
    }

    public const int SIZE = 4;

    public static readonly ILOffset Unknown = new(int.MinValue);
    
    private readonly int _offset;

    public bool IsUnknown => _offset < 0;

    public ILOffset(int offset)
    {
        if (offset < 0)
        {
            _offset = int.MinValue;
        }
        else
        {
            _offset = offset;
        }
    }

    public void RenderTo(TextBuilder builder)
    {
        builder.Append("IL_")
            .If(_offset, static off => off >= 0,
                static (tb, off) => tb.Format(off, "X4"),
                static (tb, _) => tb.Append("????"));
    }

    public int CompareTo(ILOffset other)
    {
        // Unknown is less than everything other than Unknown
        if (IsUnknown)
        {
            if (other.IsUnknown)
                return 0;
            return -1;
        }
        else
        {
            if (other.IsUnknown)
                return 1;
            return _offset.CompareTo(other._offset);
        }
    }


    public bool Equals(ILOffset ilOffset)
    {
        if (IsUnknown)
            return ilOffset.IsUnknown;
        return ilOffset._offset == _offset;
    }

    public bool Equals(int offset)
    {
        if (IsUnknown)
            return offset < 0;
        return offset == _offset;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ILOffset ilOffset)
            return Equals(ilOffset);
        if (obj is int offset)
            return Equals(offset);
        return false;
    }
    public override int GetHashCode() => _offset;

    public override string ToString() => TextBuilder.Build(RenderTo);
}