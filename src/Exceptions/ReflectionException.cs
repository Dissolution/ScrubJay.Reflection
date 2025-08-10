using ScrubJay.Collections.NonGeneric;

namespace ScrubJay.Reflection.Exceptions;

[PublicAPI]
public class ReflectionException : Exception
{
    private static readonly Action<Exception, string> _setExceptionMessage;
    
    static ReflectionException()
    {
        // Message Setter
        var messageField = typeof(Exception)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(field => TextHelper.Contains(field.Name, "message", StringComparison.OrdinalIgnoreCase))
            .OneOrDefault()
            .ThrowIfNull("Could not find Exception._message field");

        var dyn = Runtime.Builder.CreateDynamicMethod($"set_{messageField.Name}", typeof(void), [typeof(Exception), typeof(string)]);
        var gen = dyn.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, messageField);
        gen.Emit(OpCodes.Ret);
        _setExceptionMessage = dyn.CreateDelegate<Action<Exception, string>>();
    }
    
    
    public new string Message
    {
        get => base.Message;
        set => _setExceptionMessage(this, value);
    }

    public new IDictionary<string, object?> Data
    {
        get => new DictionaryAdapter<string, object?>(base.Data);
    }
    
    public ReflectionException() : base() { }
    
    public ReflectionException(ref InterpolatedTextBuilder message) 
        : base(message.ToStringAndDispose()) { }
    
    public ReflectionException(ref InterpolatedTextBuilder message, Exception? innerException) 
        : base(message.ToStringAndDispose(), innerException) { }
    
    public ReflectionException(string? message) 
        : base(message) { }
    
    public ReflectionException(string? message, Exception? innerException) 
        : base(message, innerException) { }
}