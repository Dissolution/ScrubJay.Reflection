using ScrubJay.Collections;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection;

/// <summary>
/// An <see cref="Exception"/> thrown during Jayflect operations
/// </summary>
public class Reflexception : Exception
{
    private static readonly Action<Exception, string?> _setExceptionMessage;
    private static readonly Action<Exception, Exception?> _setExceptionInnerException;

    static Reflexception()
    {
        var exceptionMessageField = Mirror.Search<Exception>
            .TryFindMember<FieldInfo>(b => b
                .Field
                .NonPublic
                .Instance
                .Name("_message")
                .FieldType<string>())
            .OkOrThrow();

//        _setExceptionMessage = RuntimeBuilder.CreateDelegate<Action<Exception, string?>>(
//            "Exception_set_Message",
//            emitter => emitter
//                .Ldarg(0)
//                .Ldarg(1)
//                .Stfld(exMessageField)
//                .Ret());

        var exceptionInnerExceptionField = Mirror.Search<Exception>
            .TryFindMember<FieldInfo>(b =>b
                .Field
                .NonPublic
                .Instance
                .Name("_innerException")
                .FieldType<Exception>())
            .OkOrThrow();
      
//        _setExceptionInnerException = RuntimeBuilder.CreateDelegate<Action<Exception, Exception?>>(
//            "Exception_set_InnerException",
//            emitter => emitter
//                .Ldarg(0)
//                .Ldarg(1)
//                .Stfld(exInnerExField)
//                .Ret());
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

    public Reflexception()
        : base()
    {

    }
    
    public Reflexception(
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException)
    {

    }

    public override string ToString()
    {
        return base.ToString();
    }
}