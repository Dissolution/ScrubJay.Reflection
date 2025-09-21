namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class MemberInfoExtensions
{
    extension(MemberInfo)
    {
    }

    extension(MemberInfo? member)
    {
        public bool IsStatic() => member switch
        {
            MethodBase method => method.IsStatic,
            FieldInfo fieldInfo => fieldInfo.IsStatic,
            PropertyInfo propertyInfo => IsStatic(propertyInfo.GetMethod) || IsStatic(propertyInfo.SetMethod),
            EventInfo eventInfo => IsStatic(eventInfo.AddMethod) || IsStatic(eventInfo.RemoveMethod) ||
                                   IsStatic(eventInfo.RaiseMethod),
            Type type => type is { IsAbstract: true, IsSealed: true },
            _ => false,
        };

        [NotNullIfNotNull(nameof(member))]
        public Type? OwnerType
        {
            get
            {
                if (member is null)
                    return null;
                return member.DeclaringType ?? member.ReflectedType ?? member.Module.GetType();
            }
        }

        public Viz Visibility
        {
            get
            {
                return member switch
                {
                    FieldInfo fieldInfo => fieldInfo.Visibility,
                    PropertyInfo propertyInfo => propertyInfo.Visibility,
                    EventInfo eventInfo => eventInfo.Visibility,
                    ConstructorInfo ctor => ctor.Visibility,
                    MethodInfo method => method.Visibility,
                    MethodBase methodBase => methodBase.Visibility,
                    Type type => type.Visibility,
                    _ => Viz.None,
                };
            }
        }

        public Attribute[] GetAttributes(bool inherit = true)
        {
            if (member is null) return [];
            return Attribute.GetCustomAttributes(member, inherit);
        }

        public Type[] GetGenericTypes()
        {
            return member switch
            {
                MethodBase method => method.GetGenericArguments(),
                Type type => type.GetGenericArguments(),
                _ => [],
            };
        }
    }
}