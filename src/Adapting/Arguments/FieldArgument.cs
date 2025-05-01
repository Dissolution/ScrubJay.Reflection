using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public sealed class FieldArgument : Argument,
#if NET7_0_OR_GREATER
    IEqualityOperators<FieldArgument, FieldArgument, bool>,
#endif
    IEquatable<FieldArgument>
{
    public static implicit operator FieldArgument(FieldInfo field) 
        => new FieldArgument(field);

    public static bool operator ==(FieldArgument? left, FieldArgument? right)
        => Equate.EquatableValues(left, right);

    public static bool operator !=(FieldArgument? left, FieldArgument? right)
        => !Equate.EquatableValues(left, right);


    public FieldInfo Field { get; }
    
    public FieldArgument(FieldInfo field) : base(field.FieldType)
    {
        this.Field = field;
        if (this.IsByRef())
            Debugger.Break();
    }

    public override Emitter Load(Emitter emitter)
    {
        if (Field.IsStatic)
        {
            return emitter.Ldsfld(Field);
        }
        else
        {
            return emitter.Ldfld(Field);
        }
    }

    public override Emitter LoadAddr(Emitter emitter)
    {
        if (Field.IsStatic)
        {
            return emitter.Ldsflda(Field);
        }
        else
        {
            return emitter.Ldflda(Field);
        }
    }

    public override Emitter Store(Emitter emitter)
    {
        if (Field.IsStatic)
        {
            return emitter.Stsfld(Field);
        }
        else
        {
            return emitter.Stfld(Field);
        }
    }

 

    public bool Equals(FieldArgument? other) => other is not null && other.Field == this.Field;

    public override bool Equals(Argument? other) => other is FieldArgument fieldArg && Equals(fieldArg);

    public override bool Equals(object? obj) => obj is FieldArgument fieldArg && Equals(fieldArg);

    public override int GetHashCode() => Hasher.HashMany(typeof(FieldArgument), Type, Field);
    
    public override void RenderTo<B>(B builder) => builder.Render(Field);
}