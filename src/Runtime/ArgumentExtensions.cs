using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.IL.Emission.Arguments;

namespace ScrubJay.Reflection.Runtime;

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