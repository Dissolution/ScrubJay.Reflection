namespace ScrubJay.Sigil.Impl;

internal delegate LocalBuilder DeclareLocalDelegate(ILGenerator il);


internal delegate System.Reflection.Emit.Label DefineLabelDelegate(System.Reflection.Emit.ILGenerator il);


internal delegate void LocalReusableDelegate(SigilLocal sigilLocal);

/// <summary>
/// This is a placeholder type used from EmitNonGeneric to take the place of DelegateType in proper Emit.
/// </summary>
internal delegate void NonGenericPlaceholderDelegate();


internal delegate void UpdateOpCodeDelegate(System.Reflection.Emit.OpCode newOpcode);
