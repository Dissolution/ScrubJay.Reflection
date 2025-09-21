#if !NET8_0_OR_GREATER

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved for use by a compiler for tracking metadata.
    /// This attribute should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface |
        AttributeTargets.Delegate, Inherited = false)]
    public sealed class NullableContextAttribute : Attribute
    {
        /// <summary>Flag specifying metadata related to nullable reference types.</summary>
        public readonly byte Flag;

        /// <summary>Initializes the attribute.</summary>
        /// <param name="value">The flag value.</param>
        public NullableContextAttribute(byte value)
        {
            Flag = value;
        }
    }
    
    /// <summary>
    /// Reserved for use by a compiler for tracking metadata.
    /// This attribute should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
    public sealed class NullableAttribute : Attribute
    {
        /// <summary>Flags specifying metadata related to nullable reference types.</summary>
        public readonly byte[] NullableFlags;

        /// <summary>Initializes the attribute.</summary>
        /// <param name="value">The flags value.</param>
        public NullableAttribute(byte value)
        {
            NullableFlags = [value];
        }

        /// <summary>Initializes the attribute.</summary>
        /// <param name="value">The flags value.</param>
        public NullableAttribute(byte[] value)
        {
            NullableFlags = value;
        }
    }
}
#endif

namespace ScrubJay.Reflection.Utilities
{

// https://blog.maartenballiauw.be/post/2022/04/19/internals-of-csharp-nullable-reference-types-migrating-to-nullable-reference-types-part-2.html

    public enum NullableContext
    {
        /// <summary>
        /// Use the default, pre-C#8 behaviour<br/>
        /// <i>(everything is maybe null)</i>
        /// </summary>
        Oblivious = 0,

        /// <summary>
        /// Consider the scope as not annotated by default<br/>
        /// <i>(every reference type is non-nullable by default)</i>
        /// </summary>
        NotAnnotated = 1,

        /// <summary>
        /// Consider the scope as annotated by default<br/>
        /// <i>(every reference type has an implicit ?)</i>
        /// </summary>
        Annotated = 2,
    }
}