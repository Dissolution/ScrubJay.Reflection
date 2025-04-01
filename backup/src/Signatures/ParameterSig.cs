using PA = System.Reflection.ParameterAttributes;

namespace ScrubJay.Reflection.Signatures;

public record class ParameterSig
{
    [return: NotNullIfNotNull(nameof(parameter))]
    public static implicit operator ParameterSig?(ParameterInfo? parameter) => parameter is null ? null : new(parameter);


    public Attributes Attributes { get; } = [];

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// <c>-1</c> is a <c>return</c> parameter, <c>0..</c> are argument parameters
    /// </remarks>
    public int? Index { get; set; }
    public string? Name { get; set; }
    public Type? Type { get; set; }
    public ReferenceKind RefKind { get; set; } = ReferenceKind.Default;
    public Option<object?> Default { get; set; } = Option<object?>.None();

    public PA ParameterAttributes
    {
        get
        {
            var attr = PA.None;
            var refKind = RefKind;
            if (refKind.HasFlags(ReferenceKind.In))
                attr.AddFlag(PA.In);
            if (refKind.HasFlags(ReferenceKind.Out))
                attr.AddFlag(PA.Out);
            if (Index == -1)
                attr.AddFlag(PA.Retval);
            if (Default.IsSome())
                attr.AddFlag(PA.Optional | PA.HasDefault);
            return attr;
        }
    }

    public Type? CompositeType
    {
        get
        {
            if (Type is null)
                return null;
            if (RefKind == ReferenceKind.Default)
                return Type;
            return Type.MakeByRefType();
        }
    }

    public ParameterSig() { }

    public ParameterSig(ParameterInfo parameterInfo)
    {
        this.Attributes = new Attributes(Attribute.GetCustomAttributes(parameterInfo));
        this.Index = parameterInfo.Position;
        this.Name = parameterInfo.Name;
        this.RefKind = parameterInfo.ReferenceKind(out var type);
        this.Type = type;
        if (parameterInfo.HasDefaultValue)
        {
            this.Default = Some<object?>(parameterInfo.DefaultValue);
        }
    }
}

public class Parameters : IReadOnlyCollection<ParameterSig>
{
    public static implicit operator Parameters(ParameterInfo[] parameterInfos) => new(parameterInfos);

    private List<ParameterSig> _parameters = [];

    public int Count => _parameters.Count;

    public Parameters() { }

    public Parameters(params ParameterInfo[] parameterInfos)
    {
        int count = parameterInfos.Length;
        _parameters = new(count);
        for (int i = 0; i < count; i++)
        {
            _parameters.Add(new(parameterInfos[i]));
        }
    }

    public void Add(ParameterInfo parameterInfo)
    {
        _parameters.Add(new(parameterInfo));
    }

    public void Add(ParameterSig parameterSig)
    {
        _parameters.Add(parameterSig);
    }

    public Type[] CompositeTypes()
    {
        var parameters = _parameters;
        int count = parameters.Count;
        var types = new Type[count];
        for (int i = 0; i < count; i++)
        {
            types[i] = parameters[i].CompositeType.ThrowIfNull();
        }
        return types;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<ParameterSig> GetEnumerator() => _parameters.GetEnumerator();
}
