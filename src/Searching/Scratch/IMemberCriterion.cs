using System.CodeDom;
using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;

using ScrubJay.Collections;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching.Scratch;

public interface IMemberCriterion : IAttributesCriterion
{
    
}

public interface IMemberCriterion<in TMember> : IMemberCriterion,
    ICriterion<TMember>
    where TMember : MemberInfo
{
    Vis Visibility { get; set; }
    Acc Access { get; set; }
    MemberTypes MemberTypes { get; set; }
    ICriterion<string>? Name { get; set; }
}

public record class MemberCriterion<TMember> : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    public Visibility Visibility { get; set; } = Visibility.Any;
    public Access Access { get; set; } = Access.Any;
    public MemberTypes MemberTypes { get; set; } = MemberTypes.All;
    public ICriterion<string>? Name { get; set; } = PassCriterion<string>.Any;

    public TypeSet? RequiredAttributes { get; set; } = null;

    internal BindingFlags BindingFlags
    {
        get
        {
            BindingFlags flags = default;
            if (Visibility.HasAnyFlags(Visibility.Private, Visibility.Protected, Visibility.Internal))
                flags |= BindingFlags.NonPublic;
            if (Visibility.HasFlags(Visibility.Public))
                flags |= BindingFlags.Public;
            if (Access.HasFlags(Access.Static))
                flags |= BindingFlags.Static;
            if (Access.HasFlags(Access.Instance))
                flags |= BindingFlags.Instance;
            return flags;
        }
    }

    public MemberCriterion() : base() { }
    internal MemberCriterion(IMemberCriterion<TMember> criterion)
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

        if (Name is not null && !Name.Matches(member.Name))
            return false;

        if (RequiredAttributes is not null && RequiredAttributes.Count > 0)
        {
            var attributes = Attribute.GetCustomAttributes(member);
            if (!RequiredAttributes.All(reqAttrType => attributes.Any(attr => attr.GetType() == reqAttrType)))
                return false;
        }

        return true;
    }
}

public interface IMemberCriterionBuilder<out TBuilder, TCriterion, TMember>
    where TBuilder : IMemberCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    TBuilder Visibility(Vis visibility);
    TBuilder Public { get; }
    TBuilder NonPublic { get; }
    
    TBuilder Access(Acc access);
    TBuilder Instance { get; }
    TBuilder Static { get; }
    
    TBuilder Name(ICriterion<string> nameCriterion);
    TBuilder Name(string name, StringMatch nameName = StringMatch.Exact, StringComparison nameComparison = StringComparison.Ordinal);
    
    TBuilder MemberTypes(MemberTypes memberTypes);
    
    /* Individual member types
     * named with 'is' to avoid .Type conflicts
     */
    TBuilder IsField { get; }
    TBuilder IsProperty { get; }
    TBuilder IsEvent { get; }
    TBuilder IsConstructor { get; }
    TBuilder IsMethodBase { get; }
    TBuilder IsMethod { get; }
    ITypeCriterionBuilderImpl IsType { get; }

}

internal class MemberCriterionBuilder<TBuilder, TCriterion, TMember> : 
    IMemberCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMemberCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMemberCriterion<TMember>
    where TMember : MemberInfo
{
    protected readonly TBuilder _builder;
    protected TCriterion _criterion;

    internal TCriterion GetCriterion() => _criterion;
    
    protected MemberCriterionBuilder()
    {
        _builder = (TBuilder)(IMemberCriterionBuilder<TBuilder, TCriterion, TMember>)this;
        _criterion = (TCriterion)(IMemberCriterion<TMember>)new MemberCriterion<TMember>();
    }
    protected MemberCriterionBuilder(TCriterion criterion)
    {
        _builder = (TBuilder)(IMemberCriterionBuilder<TBuilder, TCriterion, TMember>)this;
        _criterion = criterion;
    }

    public TBuilder Visibility(Vis visibility)
    {
        _criterion.Visibility = visibility;
        return _builder;
    }
    public TBuilder Public => Visibility(Vis.Public);
    public TBuilder NonPublic => Visibility(Vis.NonPublic);
    
    public TBuilder Access(Acc access)
    {
        _criterion.Access = access;
        return _builder;
    }
    public TBuilder Instance => Access(Acc.Instance);
    public TBuilder Static => Access(Acc.Static);
    
    public TBuilder Name(ICriterion<string> nameCriterion)
    {
        _criterion.Name = nameCriterion;
        return _builder;
    }
    public TBuilder Name(string name, StringMatch nameName = StringMatch.Exact, StringComparison nameComparison = StringComparison.Ordinal)
        => Name(new StringMatchCriterion { String = name, StringMatch = nameName, StringComparison = nameComparison });
    
    public TBuilder MemberTypes(MemberTypes memberTypes)
    {
        _criterion.MemberTypes = memberTypes;
        return _builder;
    }

    public TBuilder IsField => throw new NotImplementedException();
    public TBuilder IsProperty => throw new NotImplementedException();
    public TBuilder IsEvent => throw new NotImplementedException();
    public TBuilder IsConstructor => throw new NotImplementedException();
    public TBuilder IsMethodBase => throw new NotImplementedException();
    public TBuilder IsMethod => throw new NotImplementedException();
    public ITypeCriterionBuilderImpl IsType => new TypeCriterionBuilderImpl(new TypeCriterion((IMemberCriterion<MemberInfo>)_criterion));
}




public interface IMemberCriterionBuilderImpl : 
    IMemberCriterionBuilder<IMemberCriterionBuilderImpl, IMemberCriterion<MemberInfo>, MemberInfo>;

public class MemberCriterionBuilderImpl : 
    MemberCriterionBuilder<IMemberCriterionBuilderImpl, IMemberCriterion<MemberInfo>, MemberInfo>,
    IMemberCriterionBuilderImpl
{
    
}