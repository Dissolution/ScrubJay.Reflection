namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TypeExtensions
{
    extension(Type)
    {
    }

    extension(Type? type)
    {
        public Viz Visibility
        {
            get
            {
                var visibility = Viz.None;
                if (type is null)
                    return visibility;
                if (type.IsStatic())
                {
                    visibility |= Viz.Static;
                }
                else
                {
                    visibility |= Viz.Instance;
                }

                if (IsPublic(type))
                    visibility |= Viz.Public;
                if (IsInternal(type))
                    visibility |= Viz.Internal;
                if (IsProtected(type))
                    visibility |= Viz.Protected;
                if (IsPrivate(type))
                    visibility |= Viz.Private;
                return visibility;
            }
        }
    }

    public static bool IsNullOrVoid(this Type? type)
    {
        return type is null || type == typeof(void);
    }

    private static bool IsPublic(Type type)
    {
        // public types are visible
        return type.IsVisible &&
               (type.IsPublic || (type.IsNested && type.IsNestedPublic));
    }

    private static bool IsInternal(Type type)
    {
        return type.IsNotPublic ||
               (type.IsNested && (type.IsNestedAssembly || type.IsNestedFamORAssem || type.IsNestedFamANDAssem));
    }

    // only nested types can be declared "protected"
    private static bool IsProtected(Type type)
    {
        return type.IsNested && (type.IsNestedFamily || type.IsNestedFamORAssem || type.IsNestedFamANDAssem);
    }

    // only nested types can be declared "private"
    private static bool IsPrivate(Type type)
    {
        return type.IsNested && type.IsNestedPrivate;
    }
}