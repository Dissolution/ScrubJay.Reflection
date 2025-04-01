
namespace ScrubJay.Sigil.Impl;

/// <summary>
/// This type represents a provably null value on the stack.
///
/// Nulls typically arrive on the stack via LoadNull.
///
/// Null can be assigned to any reference type safely, without the need for a CastClass.
///
/// This type is exposed to allow for stack assertions containing null via Emit.MarkLabel.
/// </summary>
internal sealed class NullType
{
    private NullType() { }
}

// Represents a type that *could be* anything
internal sealed class WildcardType { }

// Represents *any* pointer
internal sealed class AnyPointerType { }

// Represents *any* & type
internal sealed class AnyByRefType { }

// Something that's *only* assignable from object
internal sealed class OnlyObjectType { }

// Something that means "pop the entire damn stack" when encountered by the verifier

// Something that represents a * type that cannot stand on it's own, but be inferred from where it's used
internal sealed class SamePointerType { }
internal sealed class SameByRefType { }
