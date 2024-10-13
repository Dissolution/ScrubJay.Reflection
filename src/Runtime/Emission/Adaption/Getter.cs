namespace ScrubJay.Reflection.Runtime.Emission.Adaption;

public delegate TValue Getter<TInstance, out TValue>(ref TInstance? instance);