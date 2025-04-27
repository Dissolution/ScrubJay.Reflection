namespace ScrubJay.Reflection.Adapting;

public delegate T Getter<I, out T>([AllowNull, Instance] ref I instance);

public delegate void Setter<I, in T>([AllowNull, Instance] ref I instance, T value);

public delegate void AddHandler<I, in H>([AllowNull, Instance] ref I instance, H handler)
    where H : Delegate;

public delegate void RemoveHandler<I, in H>([AllowNull, Instance] ref I instance, H handler)
    where H : Delegate;

public delegate void RaiseHandler<I>([AllowNull, Instance] ref I instance, params object?[] eventArgs);