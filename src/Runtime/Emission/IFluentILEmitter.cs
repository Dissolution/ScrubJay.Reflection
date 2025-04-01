namespace ScrubJay.Reflection.Runtime.Emission;

/// <summary>
/// An ILEmitter emits IL Instructions to a Stream
/// </summary>
/// <typeparam name="TEmitter"></typeparam>
public interface IFluentILEmitter<TEmitter>
    where TEmitter : IFluentILEmitter<TEmitter>
{

}