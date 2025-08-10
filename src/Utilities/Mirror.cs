namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public class Mirror
{
    public static class Flags
    {
        public const BindingFlags All = BindingFlags.Instance | BindingFlags.Static |
                                        BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.IgnoreCase;

        public const BindingFlags Instance = BindingFlags.Instance |
                                             BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.IgnoreCase;

        public const BindingFlags Static = BindingFlags.Static |
                                           BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.IgnoreCase;

        public const BindingFlags Public = BindingFlags.Instance | BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.IgnoreCase;

        public const BindingFlags NonPublic = BindingFlags.Instance | BindingFlags.Static |
                                              BindingFlags.NonPublic |
                                              BindingFlags.IgnoreCase;
    }
}