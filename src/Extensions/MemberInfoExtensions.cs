using Vis = ScrubJay.Reflection.Visibility;

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
}