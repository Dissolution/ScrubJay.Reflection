namespace ScrubJay.Reflection.Cloning;

/// <summary>
/// A delegate that represents the deep cloning of a value reference
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <paramref name="value"/> to be deep cloned
/// </typeparam>
/// <param name="value">
/// A readonly reference to the value to be deep cloned
/// </param>
/// <returns>
/// A deep clone of <paramref name="value"/>
/// </returns>
/// <remarks>
/// A deep clone traverses the entire object graph to create a clone that contains no references to the source value
/// </remarks>
[return: NotNullIfNotNull(nameof(value))]
public delegate T? DeepClone<T>(ref readonly T? value);