using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;
using BF = System.Reflection.BindingFlags;

namespace ScrubJay.Reflection.Searching.Criteria;

public interface IMemberCriterion : IHasAttributesCriterion
{
    Vis Visibility { get; set; }
    Acc Access { get; set; }
    MemberTypes MemberTypes { get; set; }
    ICriterion<string> Name { get; set; }
    
    ICriterion<Type> DeclaringType { get; set; }
}

public static class MemberCriterionExtensions
{
    public static BF BindingFlags(this IMemberCriterion criterion)
    {
        var (vis, acc) = (criterion.Visibility, criterion.Access);
        BF flags = default;
        if (vis.HasAnyFlags(Vis.Private, Vis.Protected, Vis.Internal))
            flags |= BF.NonPublic;
        if (vis.HasFlags(Vis.Public))
            flags |= BF.Public;
        if (acc.HasFlags(Acc.Static))
            flags |= BF.Static;
        if (acc.HasFlags(Acc.Instance))
            flags |= BF.Instance;
        return flags;
    }
}

public interface IMemberCriterion<in TMember> : IMemberCriterion, ICriterion<TMember>
    where TMember : MemberInfo
{

}

public record class MemberCriterion<TMember> : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    public Visibility Visibility { get; set; } = Visibility.Any;
    public Access Access { get; set; } = Access.Any;
    public MemberTypes MemberTypes { get; set; } = MemberTypes.All;
    public ICriterion<string> Name { get; set; } = Criterion<string>.Pass;

    public ICriterion<Attribute[]> Attributes { get; set; } = Criterion<Attribute[]>.Pass;
    public ICriterion<Type> DeclaringType { get; set; } = Criterion<Type>.Pass;

    public MemberCriterion() : base() { }

    internal MemberCriterion(IMemberCriterion criterion)
    {
        this.Visibility = criterion.Visibility;
        this.Access = criterion.Access;
        this.MemberTypes = criterion.MemberTypes;
        this.Name = criterion.Name;
    }

    public virtual bool Matches([NotNullWhen(true)] TMember? member)
    {
        // By default, we do not pass on null
        if (member is null)
            return false;

        if (!Visibility.HasAnyFlags(member.Visibility()))
            return false;

        if (!Access.HasAnyFlags(member.Access()))
            return false;

        if (!MemberTypes.HasAnyFlags(member.MemberType))
            return false;

        if (!Name.Matches(member.Name))
            return false;
        
        var attributes = Attribute.GetCustomAttributes(member);
        if (!Attributes.Matches(attributes))
            return false;

        if (!DeclaringType.Matches(member.DeclaringType))
            return false;

        return true;
    }
}

public interface IMemberBaseCriterionBuilder<out TBuilder, TCriterion, in TMember> : 
    IHasAttributesCriterionBuilder<TBuilder>,
    ICriterion<TMember>
    where TBuilder : IMemberBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    TBuilder Visibility(MemberInfo member);
    TBuilder Visibility(Vis visibility);
    TBuilder Public { get; }
    TBuilder NonPublic { get; }
    TBuilder Internal { get; }
    TBuilder Protected { get; }
    TBuilder Private { get; }

    TBuilder Access(MemberInfo member);
    TBuilder Access(Acc access);
    TBuilder Instance { get; }
    TBuilder Static { get; }

    TBuilder BindingFlags(BF flags);
    
    TBuilder Name(MemberInfo member);
    TBuilder Name(ICriterion<string> name);
    TBuilder Name(string name, StringMatch match = StringMatch.Exact, StringComparison comparison = StringComparison.Ordinal);

    TBuilder DeclaredIn(ICriterion<Type> type);
    TBuilder DeclaredIn(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder DeclaredIn<T>(TypeMatch match = TypeMatch.Exact);

    TBuilder Like(MemberInfo member);
}

public interface IMemberCriterionBuilder<out TBuilder, TCriterion, in TMember> : IMemberBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMemberBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    TBuilder MemberTypes(MemberTypes memberTypes);

    /* Individual member types
     * named with 'is' to avoid .Type conflicts
     */
    IFieldCriterionBuilderImpl IsField { get; }
    IPropertyCriterionBuilderImpl IsProperty { get; }
    IEventCriterionBuilderImpl IsEvent { get; }
    IConstructorCriterionBuilderImpl IsConstructor { get; }
    IMethodBaseCriterionBuilderImpl IsMethodBase { get; }
    IMethodCriterionBuilderImpl IsMethod { get; }
    ITypeCriterionBuilderImpl IsType { get; }
}

internal class MemberCriterionBuilder<TBuilder, TCriterion, TMember> :
    IMemberCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMemberBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    protected readonly TBuilder _builder;
    protected TCriterion _criterion;

    internal TCriterion GetCriterion()
    {
        return _criterion;
    }

    protected MemberCriterionBuilder(TCriterion criterion)
    {
        _builder = (TBuilder)(IMemberCriterionBuilder<TBuilder, TCriterion, TMember>)this;
        _criterion = criterion;
    }

    public bool Matches(TMember? member)
    {
        return _criterion.Matches(member);
    }

    public TBuilder Visibility(MemberInfo member)
    {
        _criterion.Visibility = member.Visibility();
        return _builder;
    }

    public TBuilder Visibility(Vis visibility)
    {
        _criterion.Visibility = visibility;
        return _builder;
    }
    public TBuilder Public
    {
        get
        {
            return Visibility(Vis.Public);
        }
    }

    public TBuilder NonPublic
    {
        get
        {
            return Visibility(Vis.NonPublic);
        }
    }

    public TBuilder Internal
    {
        get
        {
            return Visibility(Vis.Internal);
        }
    }

    public TBuilder Protected
    {
        get
        {
            return Visibility(Vis.Protected);
        }
    }

    public TBuilder Private
    {
        get
        {
            return Visibility(Vis.Private);
        }
    }

    public TBuilder Access(MemberInfo member)
    {
        _criterion.Access = member.Access();
        return _builder;
    }

    public TBuilder Access(Acc access)
    {
        _criterion.Access = access;
        return _builder;
    }
    public TBuilder Instance
    {
        get
        {
            return Access(Acc.Instance);
        }
    }

    public TBuilder Static => Access(Acc.Static);

    public TBuilder BindingFlags(BF flags)
    {
        _criterion.Access = flags.Access();
        _criterion.Visibility = flags.Visibility();
        return _builder;
    }

    public TBuilder Name(MemberInfo member)
    {
        _criterion.Name = Criterion.Match(member.Name);
        return _builder;
    }
    public TBuilder Name(ICriterion<string> nameCriterion)
    {
        _criterion.Name = nameCriterion;
        return _builder;
    }
    public TBuilder Name(string name, StringMatch match = StringMatch.Exact, StringComparison comparison = StringComparison.Ordinal)
    {
        return Name(Criterion.Match(name, match, comparison));
    }

    public TBuilder HasAttribute(Type attributeType)
    {
        _criterion.Attributes = new FuncCriterion<IReadOnlyList<Attribute>>(
            attrs => attrs is not null && attrs.Any(a => a.GetType() == attributeType));
        return _builder;
    }
    public TBuilder HasAttribute<TAttribute>() where TAttribute : Attribute
        => HasAttribute(typeof(TAttribute));
    
    public TBuilder DeclaredIn(ICriterion<Type> type)
    {
        _criterion.DeclaringType = type;
        return _builder;
    }
    public TBuilder DeclaredIn(Type type, TypeMatch match = TypeMatch.Exact)
    {
        return DeclaredIn(Criterion.Match(type, match));
    }
    public TBuilder DeclaredIn<T>(TypeMatch match = TypeMatch.Exact)
    {
        return DeclaredIn(Criterion.Match(typeof(T), match));
    }

    public TBuilder Like(MemberInfo member)
    {
        _criterion.Visibility = member.Visibility();
        _criterion.Access = member.Access();
        _criterion.MemberTypes = member.MemberType;
        _criterion.Name = Criterion.Match(member.Name);
        return _builder;
    }

    public TBuilder MemberTypes(MemberTypes memberTypes)
    {
        _criterion.MemberTypes = memberTypes;
        return _builder;
    }

    public IFieldCriterionBuilderImpl IsField
    {
        get
        {
            return new FieldCriterionBuilderImpl(_criterion);
        }
    }

    public IPropertyCriterionBuilderImpl IsProperty
    {
        get
        {
            return new PropertyCriterionBuilderImpl(_criterion);
        }
    }

    public IEventCriterionBuilderImpl IsEvent
    {
        get
        {
            return new EventCriterionBuilderImpl(_criterion);
        }
    }

    public IConstructorCriterionBuilderImpl IsConstructor
    {
        get
        {
            return new ConstructorCriterionBuilderImpl(_criterion);
        }
    }

    public IMethodBaseCriterionBuilderImpl IsMethodBase
    {
        get
        {
            return new MethodBaseCriterionBuilderImpl(_criterion);
        }
    }

    public IMethodCriterionBuilderImpl IsMethod
    {
        get
        {
            return new MethodCriterionBuilderImpl(_criterion);
        }
    }

    public ITypeCriterionBuilderImpl IsType
    {
        get
        {
            return new TypeCriterionBuilderImpl(_criterion);
        }
    }
}

public interface IMemberCriterionBuilderImpl :
    IMemberCriterionBuilder<IMemberCriterionBuilderImpl, IMemberCriterion<MemberInfo>, MemberInfo>;

internal class MemberCriterionBuilderImpl :
    MemberCriterionBuilder<IMemberCriterionBuilderImpl, IMemberCriterion<MemberInfo>, MemberInfo>,
    IMemberCriterionBuilderImpl
{
    public MemberCriterionBuilderImpl()
        : base(new MemberCriterion<MemberInfo>())
    {

    }
}