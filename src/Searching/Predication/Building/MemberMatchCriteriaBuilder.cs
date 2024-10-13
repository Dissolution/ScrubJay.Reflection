using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public class MemberMatchCriteriaBuilder : MemberMatchCriteriaBuilder<MemberMatchCriteriaBuilder, MemberMatchCriteria<MemberInfo>, MemberInfo>
{
    public MemberMatchCriteriaBuilder() : base()
    {
    }

    public MemberMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class MemberMatchCriteriaBuilder<TBuilder, TCriteria, TMember> :
    MatchCriteriaBuilder<TBuilder, TCriteria, TMember>
    where TBuilder : MemberMatchCriteriaBuilder<TBuilder, TCriteria, TMember>
    where TCriteria : MemberMatchCriteria<TMember>, new()
    where TMember : MemberInfo
{
    public MemberMatchCriteriaBuilder() : base(new())
    {
    }

    public MemberMatchCriteriaBuilder(TCriteria criteria) : base(criteria)
    {
    }

    public MemberMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(
        new TCriteria()
        {
            Visibility = criteria.Visibility,
            Access = criteria.Access,
            Attributes = criteria.Attributes,
            DeclaringType = criteria.DeclaringType,
            MemberTypes = criteria.MemberTypes,
            Name = criteria.Name,
        })
    {
    }

    public TBuilder Visibility(MemberInfo member)
    {
        _criteria.Visibility = member.Visibility();
        return _builder;
    }

    public TBuilder Visibility(Visibility visibility)
    {
        _criteria.Visibility = visibility;
        return _builder;
    }

    public TBuilder Public => Visibility(Reflection.Visibility.Public);

    public TBuilder NonPublic => Visibility(Reflection.Visibility.NonPublic);

    public TBuilder Internal => Visibility(Reflection.Visibility.Internal);

    public TBuilder Protected => Visibility(Reflection.Visibility.Protected);

    public TBuilder Private => Visibility(Reflection.Visibility.Private);

    public TBuilder AccessFrom(MemberInfo member)
    {
        _criteria.Access = member.Access();
        return _builder;
    }

    public TBuilder Access(Access access)
    {
        _criteria.Access = access;
        return _builder;
    }

    public TBuilder Instance => Access(Reflection.Access.Instance);

    public TBuilder Static => Access(Reflection.Access.Static);

    public TBuilder BindingFlags(BindingFlags flags)
    {
        _criteria.Access = flags.Access();
        _criteria.Visibility = flags.Visibility();
        return _builder;
    }

    public TBuilder NameFrom(MemberInfo member)
    {
        _criteria.Name = Criteria.Equals(member.Name);
        return _builder;
    }

    public TBuilder Name(ICriteria<string> nameCriteria)
    {
        _criteria.Name = nameCriteria;
        return _builder;
    }

    public TBuilder Name(string name, StringMatchType matchType = StringMatchType.Exact, StringComparison comparison = StringComparison.Ordinal)
    {
        return Name(Criteria.Equals(name, comparison, matchType));
    }

    public TBuilder HasAttribute(Type attributeType)
    {
        _criteria.Attributes = new FuncCriteria<Attribute[]>(attrs => attrs.Any(a => a.GetType() == attributeType));
        return _builder;
    }

    public TBuilder HasAttribute<TAttribute>()
        where TAttribute : Attribute
        => HasAttribute(typeof(TAttribute));

    public TBuilder DeclaredIn(ICriteria<Type> type)
    {
        _criteria.DeclaringType = type;
        return _builder;
    }

    public TBuilder DeclaredIn(Type type, TypeMatchType match = TypeMatchType.Exact)
    {
        return DeclaredIn(new TypeEqualityCriteria(type, match));
    }

    public TBuilder DeclaredIn<T>(TypeMatchType match = TypeMatchType.Exact)
    {
        return DeclaredIn(new TypeEqualityCriteria(typeof(T), match));
    }

    public TBuilder InheritFrom(MemberInfo member)
    {
        _criteria.Visibility = member.Visibility();
        _criteria.Access = member.Access();
        _criteria.MemberTypes = member.MemberType;
        _criteria.Name = Criteria.Equals(member.Name);
        return _builder;
    }

    public TBuilder MemberTypes(MemberTypes memberTypes)
    {
        _criteria.MemberTypes = memberTypes;
        return _builder;
    }

    public TypeMatchCriteriaBuilder IsType
    {
        get
        {
            var typeMatchCriteria = new TypeMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new TypeMatchCriteriaBuilder(typeMatchCriteria);
        }
    }
    
    public FieldMatchCriteriaBuilder IsField
    {
        get
        {
            var fieldMatchCriteria = new FieldMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new FieldMatchCriteriaBuilder(fieldMatchCriteria);
        }
    }

    public PropertyMatchCriteriaBuilder IsProperty 
    {
        get
        {
            var propertyMatchCriteria = new PropertyMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new PropertyMatchCriteriaBuilder(propertyMatchCriteria);
        }
    }
    

    public EventMatchCriteriaBuilder IsEvent 
    {
        get
        {
            var eventMatchCriteria = new EventMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new EventMatchCriteriaBuilder(eventMatchCriteria);
        }
    }

    public ConstructorMatchCriteriaBuilder IsConstructor
    {
        get
        {
            var constructorMatchCriteria = new ConstructorMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new ConstructorMatchCriteriaBuilder(constructorMatchCriteria);
        }
    }



    public MethodMatchCriteriaBuilder IsMethod
    {
        get
        {
            var methodMatchCriteria = new MethodMatchCriteria()
            {
                Visibility = _criteria.Visibility,
                Access = _criteria.Access,
                MemberTypes = _criteria.MemberTypes,
                Name = _criteria.Name,
                Attributes = _criteria.Attributes,
                DeclaringType = _criteria.DeclaringType,
            };
            return new MethodMatchCriteriaBuilder(methodMatchCriteria);
        }
    }
}