namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="MemberInfo"/>
/// </summary>
[PublicAPI]
public static class MemberInfoExtensions
{
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
                return TypeExtensions.Visibility(type);
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
        return member.DeclaringType ?? member.ReflectedType ?? member.Module.GetType();
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

    public static Type[]? GenericTypes(this MemberInfo? member) => member switch
    {
        null => null,
        MethodBase method => method.GetGenericArguments(),
        FieldInfo fieldInfo => null,
        PropertyInfo propertyInfo => null,
        EventInfo eventInfo => null,
        Type type => type.GetGenericArguments(),
        _ => throw new ArgumentOutOfRangeException(nameof(member)),
    };

    public static ParameterInfo[] Parameters(this MemberInfo? member)
    {
        if (member is MethodBase method)
            return method.GetParameters();
        if (member is EventInfo eventInfo)
            return eventInfo.EventHandlerType.InvokeMethod().SomeOrThrow().GetParameters();
        if (member is PropertyInfo property)
            return property.GetIndexParameters();
        return [];
    }
    
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