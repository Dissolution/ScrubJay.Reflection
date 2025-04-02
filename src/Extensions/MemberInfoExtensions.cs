namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="MemberInfo"/>
/// </summary>
[PublicAPI]
public static class MemberInfoExtensions
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();

    [return: NotNullIfNotNull(nameof(parameter))]
    public static NullabilityInfo? NullabilityInfo(this ParameterInfo? parameter)
    {
        if (parameter is null)
            return null;
        return _nullabilityInfoContext.Create(parameter);
    }

    public static NullabilityInfo? NullabilityInfo(this MemberInfo? member)
    {
        return member switch
        {
            FieldInfo field => _nullabilityInfoContext.Create(field),
            PropertyInfo property => _nullabilityInfoContext.Create(property),
            EventInfo @event => _nullabilityInfoContext.Create(@event),
            _ => null,
        };
    }

    public static Viz Visibility(this MemberInfo? member)
    {
        switch (member)
        {
            case null:
                return Viz.None;
            case EventInfo eventInfo:
            {
                Viz visibility = default;
                visibility |= Visibility(eventInfo.AddMethod);
                visibility |= Visibility(eventInfo.RemoveMethod);
                visibility |= Visibility(eventInfo.RaiseMethod);
                return visibility;
            }
            case FieldInfo fieldInfo:
            {
                Viz visibility = default;
                visibility |= fieldInfo.IsStatic ? Viz.Static : Viz.Instance;
                if (fieldInfo.IsPublic)
                    visibility |= Viz.Public;
                if (fieldInfo.IsAssembly || fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                    visibility |= Viz.Internal;
                if (fieldInfo.IsFamily || fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                    visibility |= Viz.Protected;
                if (fieldInfo.IsPrivate)
                    visibility |= Viz.Private;
                return visibility;
            }
            case MethodBase methodBase:
            {
                Viz visibility = default;
                visibility |= methodBase.IsStatic ? Viz.Static : Viz.Instance;
                if (methodBase.IsPublic)
                    visibility |= Viz.Public;
                if (methodBase.IsAssembly || methodBase.IsFamilyAndAssembly || methodBase.IsFamilyOrAssembly)
                    visibility |= Viz.Internal;
                if (methodBase.IsFamily || methodBase.IsFamilyAndAssembly || methodBase.IsFamilyOrAssembly)
                    visibility |= Viz.Protected;
                if (methodBase.IsPrivate)
                    visibility |= Viz.Private;
                return visibility;
            }
            case PropertyInfo propertyInfo:
            {
                Viz visibility = default;
                visibility |= Visibility(propertyInfo.GetMethod);
                visibility |= Visibility(propertyInfo.SetMethod);
                return visibility;
            }
            case Type type:
            {
                Viz visibility = default;
                if (type.IsPublic)
                    visibility |= Viz.Public;
                if (type.IsNotPublic)
                    visibility |= Viz.NonPublic;
                visibility |= IsStatic(type) ? Viz.Static : Viz.Instance;
                return visibility;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(member));
        }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> that owns this <see cref="MemberInfo"/>
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(member))]
    public static Type? OwnerType(this MemberInfo? member)
    {
        if (member is null)
            return null;
        return member.ReflectedType ?? member.DeclaringType ?? member.Module.GetType();
    }

    /// <summary>
    /// Is this <see cref="MemberInfo"/> <c>static</c>?
    /// </summary>
    public static bool IsStatic(this MemberInfo? member) => member switch
    {
        null => false,
        MethodBase method => method.IsStatic,
        FieldInfo fieldInfo => fieldInfo.IsStatic,
        PropertyInfo propertyInfo => IsStatic(propertyInfo.GetMethod) || IsStatic(propertyInfo.SetMethod),
        EventInfo eventInfo => IsStatic(eventInfo.AddMethod) || IsStatic(eventInfo.RemoveMethod) || IsStatic(eventInfo.RaiseMethod),
        Type type => type is { IsAbstract: true, IsSealed: true },
        _ => throw new ArgumentOutOfRangeException(nameof(member)),
    };
    
    /// <summary>
    /// Gets the <see cref="BF"/> for this <see cref="MemberInfo"/>
    /// </summary>
    public static BF BindingFlags(this MemberInfo? member)
    {
        BF flags = BF.Default;
        switch (member)
        {
            case MethodBase methodBase:
            {
                if (methodBase.IsPrivate || methodBase.IsFamily || methodBase.IsAssembly ||
                    methodBase.IsFamilyOrAssembly || methodBase.IsFamilyAndAssembly)
                {
                    flags |= BF.NonPublic;
                }

                if (methodBase.IsPublic)
                {
                    flags |= BF.Public;
                }

                flags |= methodBase.IsStatic ? BF.Static : BF.Instance;
                return flags;
            }
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsPrivate || fieldInfo.IsFamily || fieldInfo.IsAssembly ||
                    fieldInfo.IsFamilyOrAssembly || fieldInfo.IsFamilyAndAssembly)
                {
                    flags |= BF.NonPublic;
                }

                if (fieldInfo.IsPublic)
                {
                    flags |= BF.Public;
                }

                flags |= fieldInfo.IsStatic ? BF.Static : BF.Instance;
                return flags;
            }
            case PropertyInfo propertyInfo:
            {
                flags |= BindingFlags(propertyInfo.GetMethod);
                flags |= BindingFlags(propertyInfo.SetMethod);
                return flags;
            }
            case EventInfo eventInfo:
            {
                flags |= BindingFlags(eventInfo.AddMethod);
                flags |= BindingFlags(eventInfo.RemoveMethod);
                flags |= BindingFlags(eventInfo.RaiseMethod);
                return flags;
            }
            case Type type:
            {
                if (type.IsPublic)
                    flags |= BF.Public;
                if (type.IsNotPublic)
                    flags |= BF.NonPublic;
                flags |= type.IsStatic() ? BF.Static : BF.Instance;
                return flags;
            }
            default:
                return flags;
        }
    }
}