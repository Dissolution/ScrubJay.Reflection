namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class StringExtensions
{
    public static int GetMetadataToken(this string str)
    {
        var mb = typeof(string).Module as ModuleBuilder;

        if (mb is not null)
        {
#if NET8_0_OR_GREATER
            int token = mb.GetStringMetadataToken(str);
            return token;
#endif
        }

        var methods = 
        ReflectOn(typeof(string).Module)
            .Methods()
            .Instance
            .AsList();

        Debugger.Break();
        throw new NotImplementedException();
    }
}