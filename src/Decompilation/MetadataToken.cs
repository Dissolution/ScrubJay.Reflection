using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public enum TokenType : uint
{
    Module = /*                 */ 0b00000000_00000000_00000000_00000000,
    TypeRef = /*                */ 0b00000001_00000000_00000000_00000000,
    TypeDef = /*                */ 0b00000010_00000000_00000000_00000000,
    FieldDef = /*               */ 0b00000100_00000000_00000000_00000000,
    MethodDef = /*              */ 0b00000110_00000000_00000000_00000000,
    ParamDef = /*               */ 0b00001000_00000000_00000000_00000000,
    InterfaceImpl = /*          */ 0b00001001_00000000_00000000_00000000,
    MemberRef = /*              */ 0b00001010_00000000_00000000_00000000,
    CustomAttribute = /*        */ 0b00001100_00000000_00000000_00000000,
    Permission = /*             */ 0b00001110_00000000_00000000_00000000,
    Signature = /*              */ 0b00010001_00000000_00000000_00000000,
    Event = /*                  */ 0b00010100_00000000_00000000_00000000,
    Property = /*               */ 0b00010111_00000000_00000000_00000000,
    MethodImpl = /*             */ 0b00011001_00000000_00000000_00000000,
    ModuleRef = /*              */ 0b00011010_00000000_00000000_00000000,
    TypeSpec = /*               */ 0b00011011_00000000_00000000_00000000,
    Assembly = /*               */ 0b00100000_00000000_00000000_00000000,
    AssemblyRef = /*            */ 0b00100011_00000000_00000000_00000000,
    File = /*                   */ 0b00100110_00000000_00000000_00000000,
    ExportedType = /*           */ 0b00100111_00000000_00000000_00000000,
    ManifestResource = /*       */ 0b00101000_00000000_00000000_00000000,
    GenericParam = /*           */ 0b00101010_00000000_00000000_00000000,
    MethodSpec = /*             */ 0b00101011_00000000_00000000_00000000,
    GenericParamConstraint = /* */ 0b00101100_00000000_00000000_00000000,
    String = /*                 */ 0b01110000_00000000_00000000_00000000,
    Name = /*                   */ 0b01110001_00000000_00000000_00000000,
    BaseType = /*               */ 0b01110010_00000000_00000000_00000000,
}

[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly struct MetadataToken :
#if NET7_0_OR_GREATER
    IEqualityOperators<MetadataToken, MetadataToken, bool>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEquatable<MetadataToken>,
    IFormattable,
    IRenderable
{
    private const uint TOKEN_TYPE_MASK = 0b11111111_00000000_00000000_00000000;
    private const uint IDENTIFIER_MASK = 0b00000000_11111111_11111111_11111111;

    public static implicit operator MetadataToken(int token) => new(token);
    public static implicit operator int(MetadataToken token) => token.Value;

    public static bool operator ==(MetadataToken left, MetadataToken right) => left.Equals(right);
    public static bool operator !=(MetadataToken left, MetadataToken right) => !left.Equals(right);

    public static readonly MetadataToken Empty;


    [FieldOffset(0)]
    private readonly uint _token;

    public TokenType TokenType
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TokenType)(_token & TOKEN_TYPE_MASK);
    }

    public uint Identifier
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_token & IDENTIFIER_MASK);
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _token == 0U;
    }

    public int Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)_token;
    }

    public MetadataToken(int token)
    {
        _token = (uint)token;
    }

    public bool Equals(MetadataToken token) => token._token == _token;

    public bool Equals(int token) => token == _token;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        MetadataToken metadataToken => Equals(metadataToken),
        int token => Equals(token),
        _ => false
    };

    public override int GetHashCode() => (int)_token;

    public bool TryFormat(Span<char> destination, out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        return new TryFormatWriter(destination)
        {
            { _token, format, provider },
        }.Wrote(out charsWritten);
    }

    public string ToString(string? format, IFormatProvider? provider = null) => _token.ToString(format, provider);

    public TextBuilder RenderTo(TextBuilder builder) => builder
        .Render(TokenType)
        .Append('.')
        .Format(Identifier, "X6");

    public override string ToString() => TextBuilder.Build(RenderTo);
}