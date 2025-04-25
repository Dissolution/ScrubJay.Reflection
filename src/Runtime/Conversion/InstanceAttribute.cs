namespace ScrubJay.Reflection.Runtime.Conversion;

/// <summary>
/// Marker attribute to specify the parameter of a delegate is the instance
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InstanceAttribute : Attribute;