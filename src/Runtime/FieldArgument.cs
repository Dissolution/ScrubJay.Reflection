using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.IL.Emission.Arguments;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public sealed class FieldArgument : Argument,
    IEquatable<FieldArgument>
{
    public static implicit operator FieldArgument(FieldInfo field) => new FieldArgument(field);
    
    public required FieldInfo Field { get; init; }

    [SetsRequiredMembers]
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

    public override void RenderTo<B>(B builder)
    {
        builder.Render(Field);
    }

    public bool Equals(FieldArgument? other)
    {
        return other is not null &&
            other.Field == this.Field;
    }

    public override bool Equals(Argument? other)
    {
        return other is FieldArgument fieldArg &&
            Equals(fieldArg);
    }
}