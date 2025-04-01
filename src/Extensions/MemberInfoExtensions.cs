using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;
using BF = System.Reflection.BindingFlags;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="MemberInfo"/>
/// </summary>
[PublicAPI]
public static class MemberInfoExtensions
{
    private static readonly NullabilityInfoContext _nullabilityContext = new NullabilityInfoContext();
    
    public static NullabilityInfo NullabilityInfo(this ParameterInfo parameter)
    {
        return _nullabilityContext.Create(parameter);
    }

    public static NullabilityInfo? NullabilityInfo(this MemberInfo? member)
    {
        return member switch
        {
            FieldInfo field => _nullabilityContext.Create(field),
            PropertyInfo property => _nullabilityContext.Create(property),
            EventInfo @event => _nullabilityContext.Create(@event),
            _ => null,
        };
    }
    
    
    /// <summary>
    /// Gets the <see cref="Type"/> that owns this <see cref="MemberInfo"/>
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static Type OwnerType(this MemberInfo member)
    {
        return member.ReflectedType ?? member.DeclaringType ?? member.Module.GetType();
    }

    /// <summary>
    /// Is this <see cref="MemberInfo"/> <c>static</c>?
    /// </summary>
    public static bool IsStatic(this MemberInfo? member)
    {
        return member switch
        {
            MethodBase methodBase => methodBase.IsStatic,
            FieldInfo fieldInfo => fieldInfo.IsStatic,
            PropertyInfo propertyInfo => IsStatic(propertyInfo.GetMethod) || IsStatic(propertyInfo.SetMethod),
            EventInfo eventInfo => IsStatic(eventInfo.AddMethod) || IsStatic(eventInfo.RemoveMethod) || IsStatic(eventInfo.RaiseMethod),
            Type type => type is { IsAbstract: true, IsSealed: true },
            _ => false,
        };
    }

    /// <summary>
    /// Gets the <see cref="Vis"/> of this <see cref="MemberInfo"/>
    /// </summary>
    public static Vis Visibility(this MemberInfo? member)
    {
        Vis vis = Vis.None;
        switch (member)
        {
            case MethodBase methodBase:
            {
                if (methodBase.IsPrivate)
                    vis |= Vis.Private;
                if (methodBase.IsFamily)
                    vis |= Vis.Protected;
                if (methodBase.IsAssembly)
                    vis |= Vis.Internal;
                if (methodBase.IsPublic)
                    vis |= Vis.Public;
                if (methodBase.IsFamilyOrAssembly || methodBase.IsFamilyAndAssembly)
                    vis |= (Vis.Protected | Vis.Internal);
                return vis;
            }
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsPrivate)
                    vis |= Vis.Private;
                if (fieldInfo.IsFamily)
                    vis |= Vis.Protected;
                if (fieldInfo.IsAssembly)
                    vis |= Vis.Internal;
                if (fieldInfo.IsPublic)
                    vis |= Vis.Public;
                if (fieldInfo.IsFamilyOrAssembly || fieldInfo.IsFamilyAndAssembly)
                    vis |= (Vis.Protected | Vis.Internal);
                return vis;
            }
            case PropertyInfo propertyInfo:
            {
                vis |= Visibility(propertyInfo.GetMethod);
                vis |= Visibility(propertyInfo.SetMethod);
                return vis;
            }
            case EventInfo eventInfo:
            {
                vis |= Visibility(eventInfo.AddMethod);
                vis |= Visibility(eventInfo.RemoveMethod);
                vis |= Visibility(eventInfo.RaiseMethod);
                return vis;
            }
            case Type type:
            {
                if (type.IsPublic)
                    vis |= Vis.Public;
                if (type.IsNotPublic)
                    vis |= Vis.NonPublic;
                return vis;
            }
            case null:
            default:
                return vis;
        }
    }

    /// <summary>
    /// Gets the <see cref="Acc"/> of this <see cref="MemberInfo"/>
    /// </summary>
    public static Acc Access(this MemberInfo? member)
    {
        Acc acc = Acc.None;
        switch (member)
        {
            case MethodBase methodBase:
            {
                acc |= methodBase.IsStatic ? Acc.Static : Acc.Instance;
                return acc;
            }
            case FieldInfo fieldInfo:
            {
                acc |= fieldInfo.IsStatic ? Acc.Static : Acc.Instance;
                return acc;
            }
            case PropertyInfo propertyInfo:
            {
                acc |= Access(propertyInfo.GetMethod);
                acc |= Access(propertyInfo.SetMethod);
                return acc;
            }
            case EventInfo eventInfo:
            {
                acc |= Access(eventInfo.AddMethod);
                acc |= Access(eventInfo.RemoveMethod);
                acc |= Access(eventInfo.RaiseMethod);
                return acc;
            }
            case Type type:
            {
                acc |= type.IsStatic() ? Acc.Static : Acc.Instance;
                return acc;
            }
            case null:
            default:
                return acc;
        }
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
            case null:
            default:
                return flags;
        }
    }
}