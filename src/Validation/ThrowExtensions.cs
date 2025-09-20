namespace ScrubJay.Reflection.Validation;

[PublicAPI]
public static class ThrowExtensions
{
    extension(Throw)
    {
        public static void IfStatic(MemberInfo? member)
        {
            if (member is null)
                throw new ArgumentNullException(nameof(member));
            if (member.IsStatic())
                throw new ArgumentException(Build($"{member:@} cannot be static"), nameof(member));
        }
    }
}