namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface ILocalBuilder<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Declare(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    TEmitter Declare<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    TEmitter Load(EmitterLocal local, bool asAddress = false);
    TEmitter Store(EmitterLocal local);
}