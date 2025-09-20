namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public class Mirror
{
    public static class Flags
    {
        public const BindingFlags ALL = BindingFlags.Instance | BindingFlags.Static |
                                        BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.IgnoreCase;

        public const BindingFlags INSTANCE = BindingFlags.Instance |
                                             BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.IgnoreCase;

        public const BindingFlags STATIC = BindingFlags.Static |
                                           BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.IgnoreCase;

        public const BindingFlags PUBLIC = BindingFlags.Instance | BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.IgnoreCase;

        public const BindingFlags NON_PUBLIC = BindingFlags.Instance | BindingFlags.Static |
                                               BindingFlags.NonPublic |
                                               BindingFlags.IgnoreCase;
    }
}