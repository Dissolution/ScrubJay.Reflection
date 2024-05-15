using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission;

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
    /// <summary>
    /// Invoke the given <paramref name="emission"/> action on this <typeparamref name="TEmitter"/> instance
    /// </summary>
    /// <param name="emission">The <see cref="Action{T}"/> to perform on this <typeparamref name="TEmitter"/> instance</param>
    /// <returns>
    /// This <typeparamref name="TEmitter"/> instance after <paramref name="emission"/> has been invoked
    /// </returns>
    TEmitter Invoke(Action<TEmitter> emission);
    
    /// <summary>
    /// Invoke the given <paramref name="emission"/> function on this <typeparamref name="TEmitter"/> instance
    /// </summary>
    /// <param name="emission">The <see cref="Func{T,T}"/> to perform on this <typeparamref name="TEmitter"/> instance</param>
    /// <returns>
    /// This <typeparamref name="TEmitter"/> instance after <paramref name="emission"/> has been invoked
    /// </returns>
    TEmitter Invoke(Func<TEmitter, TEmitter> emission);
}