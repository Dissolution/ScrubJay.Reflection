namespace ScrubJay.Reflection.Signatures;

public record class AttributeSig
{
    public Type? Type { get; set; }

    public AttributeSig() { }

    public AttributeSig(Attribute attribute)
    {
        this.Type = attribute.GetType();
        // todo: how to get ctor args?

    }
}

public record class ConstructedAttributeSig : AttributeSig
{
    public ConstructorInfo? Constructor { get; set; }
    public object?[] Arguments { get; set; } = [];
}


public class Attributes : List<AttributeSig>
{
    public static implicit operator Attributes(Attribute[] attributes) => new(attributes);

    public Attributes() { }

    public Attributes(params Attribute[] attributes)
    {
        this.AddRange(Array.ConvertAll<Attribute, AttributeSig>(attributes, a => new(a)));
    }
}