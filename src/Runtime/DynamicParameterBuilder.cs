using System.Linq.Expressions;
using ScrubJay.Fluent;
using ScrubJay.Reflection.Expressions;
using ScrubJay.Reflection.Naming;
using ScrubJay.Reflection.Signatures;

namespace ScrubJay.Reflection.Runtime;

[PublicAPI]
public class DynamicParameterBuilder : FluentRecordBuilder<DynamicParameterBuilder, ParameterSig>
{
    public DynamicParameterBuilder In => RefKind(ReferenceKind.In);
    public DynamicParameterBuilder Out => RefKind(ReferenceKind.Out);
    public DynamicParameterBuilder Ref => RefKind(ReferenceKind.Ref);

    public DynamicParameterBuilder() : base(new()) { }

    public DynamicParameterBuilder(ParameterSig sig) : base(sig)
    {
    }

    public DynamicParameterBuilder Attribute<TA>(Expression<Func<TA>> constructAttribute)
        where TA : Attribute
    {
        if (!ExpressionParser.TryParseConstructor(constructAttribute)
                .IsSome(out var ctorArgs))
            throw new ArgumentException(null, nameof(constructAttribute));

        ConstructedAttributeSig sig = new()
        {
            Type = typeof(TA),
            Constructor = ctorArgs.Item1,
            Arguments = ctorArgs.Item2,
        };
        _record.Attributes.Add(sig);
        return _builder;
    }

    public DynamicParameterBuilder RefKind(ReferenceKind referenceKind)
    {
        _record.RefKind = referenceKind;
        return _builder;
    }

    public DynamicParameterBuilder Type(Type type)
    {
        Throw.IfNull(type);
        if (type.IsStatic())
            throw new ArgumentException("Type cannot be static", nameof(type));
        if (type.IsByRef)
        {
            _record.RefKind = ReferenceKind.Ref;
            _record.Type = type.GetElementType()!;
        }
        else
        {
            _record.Type = type;
        }
        return _builder;
    }
    public DynamicParameterBuilder Type<T>() => Type(typeof(T));

    public DynamicParameterBuilder Named(string name)
    {
        if (NameHelper.IsValidMemberName(name))
        {
            _record.Name = name;
            return _builder;
        }

        throw new ArgumentException("Invalid Parameter name", nameof(name));
    }

    public DynamicParameterBuilder Default(object? defaultValue)
    {
        _record.Default = Some(defaultValue);
        return _builder;
    }
}
