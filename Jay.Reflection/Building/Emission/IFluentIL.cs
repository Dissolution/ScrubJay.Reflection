﻿using Jay.Reflection.Building.Emission.Instructions;

namespace Jay.Reflection.Building.Emission;

public interface IFluentIL<out TSelf>
    where TSelf : IFluentIL<TSelf>
{
    /// <summary>
    /// Gets the stream of <see cref="InstructionLine"/>s emitted thus far
    /// </summary>
    InstructionStream Instructions { get; }

    internal TSelf This => ((TSelf)this);
}