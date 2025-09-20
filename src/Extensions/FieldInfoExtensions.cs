#pragma warning disable CA1822

namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class FieldInfoExtensions
{
    extension(FieldInfo)
    {
    }

    extension(FieldInfo? fieldInfo)
    {
        public Viz Visibility
        {
            get
            {
                Viz visibility = default;
                if (fieldInfo is not null)
                {
                    visibility |= fieldInfo.IsStatic ? Viz.Static : Viz.Instance;
                    if (fieldInfo.IsPublic)
                        visibility |= Viz.Public;
                    if (fieldInfo.IsAssembly || fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                        visibility |= Viz.Internal;
                    if (fieldInfo.IsFamily || fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                        visibility |= Viz.Protected;
                    if (fieldInfo.IsPrivate)
                        visibility |= Viz.Private;
                }

                return visibility;
            }
        }
    }
}