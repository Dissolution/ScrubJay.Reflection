namespace ScrubJay.Reflection.IL;

[PublicAPI]
public sealed class ILTarget :
#if NET7_0_OR_GREATER
    IEqualityOperators<ILTarget, ILTarget, bool>,
#endif
    IEquatable<ILTarget>,
    IRenderable
{
    public static bool operator ==(ILTarget? left, ILTarget? right)
        => Equate

    public static bool operator !=(ILTarget? left, ILTarget? right)
        => !left.Equals(right);


    private readonly int? _delta;
    private readonly ILOffset _offset;
    private readonly string? _name;

    public ILTarget(int delta)
    {
        _delta = delta;
        _offset = ILOffset.Unknown;
        _name = null;
    }

    public ILTarget(ILOffset offset)
    {
        if (offset.IsUnknown)
            throw new ArgumentException("ILOffset must be known", nameof(offset));
        _delta = null;
        _offset = offset;
        _name = null;
    }

    public ILTarget(string name)
    {
        Throw.IfEmpty(name);
        _delta = null;
        _offset = ILOffset.Unknown;
        _name = name;
    }

    public bool Equals(ILTarget other)
    {
        if (_label.TryGetValue(out var label))
        {
            return other._label.TryGetValue(out var otherLabel) && otherLabel == label;
        }
        else if (_delta.TryGetValue(out var delta))
        {
            return other._delta.TryGetValue(out var otherDelta) && otherDelta == delta;
        }
        else
        {
            return other._offset == _offset;
        }
    }

    public bool Equals(ILLabel label)
    {
        return _label.TryGetValue(out var lbl) && lbl == label;
    }

    public bool Equals(ILOffset offset)
    {
        return _offset == offset;
    }

    public bool Equals(int delta)
    {
        return _delta.TryGetValue(out var d) && d == delta;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            ILTarget target => Equals(target),
            ILLabel label => Equals(label),
            ILOffset offset => Equals(offset),
            int delta => Equals(delta),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.HashMany(_label, _delta, _offset);
    }

    public void RenderTo(TextBuilder builder)
    {
        if (_label.TryGetValue(out var label))
        {
            builder.Append("🏷️").Render(label);
        }
        else if (_delta.TryGetValue(out var delta))
        {
            builder.Append('Δ').Format(delta, "+#;-#;0");
        }
        else
        {
            builder.Append("🎯").Render(_offset);
        }
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}