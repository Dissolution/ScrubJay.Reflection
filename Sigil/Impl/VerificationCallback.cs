using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal delegate void VerificationCallback(LinqStack<List<TypeOnStack>> currentStack, bool isBaseless);
