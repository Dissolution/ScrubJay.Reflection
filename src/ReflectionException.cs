using ScrubJay.Collections.NonGeneric;
using ScrubJay.Debugging;
using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection;

/// <summary>
/// An <see cref="Exception"/> thrown during Reflection operations
/// </summary>
[PublicAPI]
public class ReflectionException : Exception
{
    private static readonly Action<Exception, string?> _setExceptionMessage;
    private static readonly Action<Exception, Exception?> _setExceptionInnerException;

    static ReflectionException()
    {
        var exceptionMessageField = Reflect<Exception>()
            .NonPublic.Instance.Fields<string>()
            .Named("message", new StringMatch(StringComparison.OrdinalIgnoreCase){ Contains = true})
            .OneOrThrow();

        _setExceptionMessage = RuntimeBuilder.TryEmitDelegate<Action<Exception, string?>>(emitter => emitter
                .Ldarg(0)
                .Ldarg(1)
                .Stfld(exceptionMessageField)
                .Ret())
            .OkOrThrow();

        var exceptionInnerExceptionField = Reflect<Exception>()
            .NonPublic.Instance.Fields<Exception>()
            .Named("innerException", new StringMatch(StringComparison.OrdinalIgnoreCase){ Contains = true })
            .OneOrThrow();

        _setExceptionInnerException = RuntimeBuilder.TryEmitDelegate<Action<Exception, Exception?>>(emitter => emitter
            .Ldarg(0)
            .Ldarg(1)
            .Stfld(exceptionInnerExceptionField)
            .Ret())
            .OkOrThrow();
    }
    
    public new string Message
    {
        get => base.Message;
        init => _setExceptionMessage(this, value);
    }

    public new Exception? InnerException
    {
        get => base.InnerException;
        init => _setExceptionInnerException(this, value);
    }

    private DictionaryAdapter<string, object?>? _data = null;

    public new IDictionary<string, object?> Data => _data ??= new(base.Data);

    public ReflectionException() : base(message: null) { }

    public ReflectionException(string? message) : base(message) { }
    
    public override string ToString()
    {
        return this.Dump();
    }
}