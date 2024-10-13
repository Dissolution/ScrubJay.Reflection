using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IILEmitter
{
    /// <summary>
    /// Gets the <see cref="InstructionStream">Stream</see> of <see cref="Instruction">Instructions</see> emitted thus far
    /// </summary>
    InstructionStream Instructions { get; }
}

/// <summary>
/// A fluent emitter of <see cref="Instruction">Instructions</see>
/// </summary>
/// <typeparam name="TEmitter">
/// The <see cref="Type"/> of the emitter instance returned from fluent operations
/// </typeparam>
public interface IILEmitter<TEmitter> : IILEmitter
    where TEmitter : IILEmitter<TEmitter>
{

}