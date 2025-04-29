using System.Collections.Concurrent;

namespace ScrubJay.Reflection.Exceptions;

public class ExceptionHelper
{
        private sealed record class ExceptionCtorKey(Type ExceptionType, Type[] CtorArgTypes)
    {
        public static ExceptionCtorKey Create(Type exType, params Type[] argTypes) => new(exType, argTypes);

        public static ExceptionCtorKey Create<TException>(params Type[] argTypes)
            where TException : Exception
            => new(typeof(TException), argTypes);
    }

    private static readonly ConcurrentDictionary<ExceptionCtorKey, Delegate> _exceptionCtorCache = new();

    public static TException Create<TException>()
        where TException : Exception, new()
        => Activator.CreateInstance<TException>();

    private static TDel CreateCtr<TDel>(ExceptionCtorKey key)
        where TDel : Delegate
    {
//        var ctor = Mirror.Search(key.ExceptionType)
//            .TryFindMember(b => b.IsConstructor.Instance.Parameters(key.CtorArgTypes))
//            .OkOrThrow();
//        var del = RuntimeBuilder.EmitDelegate<TDel>(emitter => emitter.LoadArgs(..key.CtorArgTypes.Length).Newobj(ctor).Ret());
//        return del;
        throw new NotImplementedException();
    }

    // public static TException Create<TException>(ref InterpolateDeeper message)
    //     where TException : Exception
    // {
    //     string msg = message.ToStringAndDispose();
    //     var construct = _exceptionCtorCache.GetOrAdd<ExceptionCtorKey, Func<string, TException>>(
    //         ExceptionCtorKey.Create<TException>(typeof(string)),
    //         CreateCtr<Func<string, TException>>);
    //     return construct(msg);
    // }
    //
    // public static TException Create<TException>(ref InterpolateDeeper message, string? paramName)
    //     where TException : Exception
    // {
    //     string msg = message.ToStringAndDispose();
    //     var construct = _exceptionCtorCache.GetOrAdd<ExceptionCtorKey, Func<string, string?, TException>>(
    //         ExceptionCtorKey.Create<TException>(typeof(string), typeof(string)),
    //         static key => CreateCtr<Func<string, string?, TException>>(key));
    //     return construct(msg, paramName);
    // }


}