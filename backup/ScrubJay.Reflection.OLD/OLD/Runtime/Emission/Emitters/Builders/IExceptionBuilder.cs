//using ScrubJay.Reflection.Searching;
//using ScrubJay.Reflection.Searching.Criteria;
//
//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//public interface IExceptionBuilder<out TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    TEmitter Throw();
//    TEmitter Throw(Type exceptionType, params object?[] args);
//    TEmitter Throw<TException>(params object?[] args) where TException : Exception;
//
//    TEmitter ReThrow();
//}
//
//internal class ExceptionBuilder<TEmitter> : BuilderBase<TEmitter>, IExceptionBuilder<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    public ExceptionBuilder(Emitter<TEmitter> emitter) : base(emitter)
//    {
//    }
//    public TEmitter Throw() => _emitter.Throw();
//    public TEmitter Throw(Type exceptionType, params object?[] args)
//    {
//        // need to verify that we can Push each arg
//        var argCriteria = new ParameterCriteria[args.Length];
//        for (var i = 0; i < args.Length; i++)
//        {
//            Type? argType = args[i]?.GetType();
//            _emitter.TryPush(argType).OkOrThrow();
//            argCriteria[i] = ParameterCriteria.Create(TypeCriteria.Create(argType, TypeMatch.Implements));
//        }
//
//        // Find a constructor that matches
//        var ctor = Mirror.Search(exceptionType)
//            .FindMembers(b => b.Instance.Constructor.Parameters(argCriteria))
//            .FirstOrDefault();
//        if (ctor is null)
//            throw new InvalidOperationException();
//        _emitter.Newobj(ctor);
//        _emitter.Throw();
//        return _emitter.AsTEmitter;
//    }
//
//    public TEmitter Throw<TException>(params object?[] args)
//        where TException : Exception
//        => Throw(typeof(TException), args);
//
//    public TEmitter ReThrow() => _emitter.Rethrow();
//}