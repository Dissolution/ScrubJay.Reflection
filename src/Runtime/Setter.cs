namespace ScrubJay.Reflection.Runtime;

public delegate void Setter<I, in T>([AllowNull] ref I instance, T value);