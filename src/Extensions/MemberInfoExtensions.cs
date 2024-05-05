using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="MemberInfo"/>
/// </summary>
public static class MemberInfoExtensions
{
    public static Vis Visibility(this MemberInfo? member)
    {
        Vis vis = Vis.None;
        switch (member)
        {
            case EventInfo eventInfo:
                vis |= Visibility(eventInfo.AddMethod);
                vis |= Visibility(eventInfo.RemoveMethod);
                vis |= Visibility(eventInfo.RaiseMethod);
                return vis;
            case FieldInfo fieldInfo:
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
            case MethodBase methodBase:
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
            case PropertyInfo propertyInfo:
                vis |= Visibility(propertyInfo.GetMethod);
                vis |= Visibility(propertyInfo.SetMethod);
                return vis;
            case Type type:
                if (type.IsPublic)
                    vis |= Vis.Public;
                if (type.IsNotPublic)
                    vis |= Vis.NonPublic;
                return vis;
            case null:
            default:
                return vis;
        }
    }

    public static Acc Access(this MemberInfo? member)
    {
        Acc acc = Acc.None;
        switch (member)
        {
            case EventInfo eventInfo:
                acc |= Access(eventInfo.AddMethod);
                acc |= Access(eventInfo.RemoveMethod);
                acc |= Access(eventInfo.RaiseMethod);
                return acc;
            case FieldInfo fieldInfo:
                acc |= fieldInfo.IsStatic ? Acc.Static : Acc.Instance;
                return acc;
            case MethodBase methodBase:
                acc |= methodBase.IsStatic ? Acc.Static : Acc.Instance;
                return acc;
            case PropertyInfo propertyInfo:
                acc |= Access(propertyInfo.GetMethod);
                acc |= Access(propertyInfo.SetMethod);
                return acc;
            case Type type:
                acc |= (type.IsAbstract && type.IsSealed) ? Acc.Static : Acc.Instance;
                return acc;
            case null:
            default:
                return acc;
        }
    }
    
}