using System.Globalization;
using ScrubJay.Memory;
using ScrubJay.Parsing;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection;

/// <summary>
/// Represents an offset in IL
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 2)]
public readonly struct ILOffset :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILOffset, ILOffset, bool>,
    IComparisonOperators<ILOffset, ILOffset, bool>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    ITrySpanParsable<ILOffset>,
    ITryParsable<ILOffset>,
    IEquatable<ILOffset>,
    IComparable<ILOffset>,
    IFormattable,
    IRenderable
{
    public static implicit operator ILOffset(int offset) => new(offset);

    public static bool operator ==(ILOffset left, ILOffset right) => left.Equals(right);
    public static bool operator !=(ILOffset left, ILOffset right) => !left.Equals(right);
    public static bool operator >(ILOffset left, ILOffset right) => left.CompareTo(right) > 0;
    public static bool operator >=(ILOffset left, ILOffset right) => left.CompareTo(right) >= 0;
    public static bool operator <(ILOffset left, ILOffset right) => left.CompareTo(right) < 0;
    public static bool operator <=(ILOffset left, ILOffset right) => left.CompareTo(right) <= 0;

    public static readonly ILOffset Unknown = new ILOffset(-1);

    public static Result<ILOffset> TryParse(text text, IFormatProvider? provider = null)
    {
        var reader = new SpanReader<char>(text);
        reader.SkipWhile(char.IsWhiteSpace);
        reader.SkipWhileMatching("IL_".AsSpan());

        if (reader.TryPeek(4).IsSome(out var four) && four.Equate("????"))
            return Unknown;

        var hex = reader.TakeWhile(ch => ch.IsAsciiHexDigit());
        reader.SkipWhile(char.IsWhiteSpace);
        if (!reader.IsCompleted)
            return ParseException<ILOffset>.Create(text);

#if NETSTANDARD2_0 || NETFRAMEWORK
        var hexStr = hex.AsString();
        
        if (short.TryParse(hexStr, NumberStyles.HexNumber, provider, out var offset))
            return new ILOffset(offset);

        if (short.TryParse(hexStr, NumberStyles.Any, provider, out offset))
            return new ILOffset(offset);
#else
        if (short.TryParse(hex, NumberStyles.HexNumber, provider, out var offset))
            return new ILOffset(offset);

        if (short.TryParse(hex, NumberStyles.Any, provider, out offset))
            return new ILOffset(offset);
#endif
        return ParseException<ILOffset>.Create(text);
    }


    [FieldOffset(0)] private readonly short _offset;

    public bool IsUnknown
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _offset == -1;
    }
    
    public ILOffset(short offset)
    {
        if (offset >= 0)
        {
            _offset = offset;
        }
        else
        {
            _offset = -1;
        }
    }

    public ILOffset(int offset)
    {
        if (offset is >= 0 and <= short.MaxValue)
        {
            _offset = (short)offset;
        }
        else
        {
            _offset = -1;
        }
    }

    public int CompareTo(ILOffset offset) => _offset.CompareTo(offset._offset);

    public bool Equals(ILOffset offset) => _offset == offset._offset;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        ILOffset ilOffset => Equals(ilOffset),
        int offset => Equals(offset),
        _ => false,
    };

    public override int GetHashCode() => _offset;

    public bool TryFormat(Span<char> destination, out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        var writer = new TryFormatWriter(destination)
        {
            "IL_",
        };

        if (_offset >= 0)
        {
            writer.Add(_offset, format, provider);
        }
        else
        {
            writer.Add("????");
        }

        return writer.Wrote(out charsWritten);
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        return TextBuilder.New
            .Append("IL_")
            .If(_offset, static o => o >= 0,
                (tb, o) => tb.Format(o, format, provider),
                static (tb, _) => tb.Append("????"))
            .ToStringAndDispose();
    }

    public void RenderTo(TextBuilder builder)
    {
        builder.Append("IL_")
            .If(_offset, static o => o >= 0,
                static (tb, o) => tb.Format(o, "X4"),
                static (tb, _) => tb.Append("????"));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}