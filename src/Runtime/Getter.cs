namespace ScrubJay.Reflection.Runtime;

public delegate T Getter<I, out T>([AllowNull] ref I instance);