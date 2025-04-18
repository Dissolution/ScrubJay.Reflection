
    // [14 5 - 14 6]
    IL_0000: nop

    // [16 9 - 16 92]
    IL_0001: ldtoken      class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>
    IL_0006: call         class [System.Runtime]System.Type [System.Runtime]System.Type::GetTypeFromHandle(valuetype [System.Runtime]System.RuntimeTypeHandle)
    IL_000b: call         object [System.Runtime]System.Runtime.CompilerServices.RuntimeHelpers::GetUninitializedObject(class [System.Runtime]System.Type)
    IL_0010: stloc.1      // uninit

    // [17 9 - 17 48]
    IL_0011: ldloc.1      // uninit
    IL_0012: castclass    class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>
    IL_0017: stloc.0      // clone

    // [19 9 - 19 65]
    IL_0018: ldloc.0      // clone
    IL_0019: ldarg.0      // tuple
    IL_001a: ldflda       !0/*int32*/ class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>::m_Item1
    IL_001f: call         !!0/*int32*/ [ScrubJay.Reflection]ScrubJay.Reflection.Cloning.Cloner::DeepClone<int32>(!!0/*int32*/&)
    IL_0024: stfld        !0/*int32*/ class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>::m_Item1

    // [20 9 - 20 68]
    IL_0029: ldloc.0      // clone
    IL_002a: ldarg.0      // tuple
    IL_002b: ldflda       !1/*string*/ class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>::m_Item2
    IL_0030: call         !!0/*string*/ [ScrubJay.Reflection]ScrubJay.Reflection.Cloning.Cloner::DeepClone<string>(!!0/*string*/&)
    IL_0035: stfld        !1/*string*/ class ScrubJay.Reflection.Sandbox.FakeTuple`2<int32, string>::m_Item2

    // [21 9 - 21 22]
    IL_003a: ldloc.0      // clone
    IL_003b: stloc.2      // V_2
    IL_003c: br.s         IL_003e

    // [22 5 - 22 6]
    IL_003e: ldloc.2      // V_2
    IL_003f: ret