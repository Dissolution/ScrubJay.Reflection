//namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;
//
//internal abstract class BuilderBase<TEmitter>
//    where TEmitter : IILEmitter<TEmitter>
//{
//    protected readonly Emitter<TEmitter> _emitter;
//
//    protected BuilderBase(Emitter<TEmitter> emitter)
//    {
//        _emitter = emitter;
//    }
//
//    protected TEmitter Emit(Action<Emitter<TEmitter>> emit)
//    {
//        emit(_emitter);
//        return (TEmitter)(IILEmitter<TEmitter>)_emitter;
//    }
//}