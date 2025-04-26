using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting.Arguments;

[PublicAPI]
public static class ArgumentExtensions
{
    public static Emitter Load(this Emitter emitter, Argument argument)
        => emitter.Invoke(argument.Load);
    
    public static Emitter LoadAddr(this Emitter emitter, Argument argument)
        => emitter.Invoke(argument.LoadAddr);
    
    public static Emitter Store(this Emitter emitter, Argument argument)
        => emitter.Invoke(argument.Store);
    
    public static Emitter LoadAsInstance(this Emitter emitter, Argument argument)
        => emitter.Invoke(argument.LoadAsInstance);

}