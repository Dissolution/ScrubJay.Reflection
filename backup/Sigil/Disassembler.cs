using ScrubJay.Sigil.Extensions;
using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil;

/// <summary>
/// Helper for disassembling delegates into a series of Emit operations.
///
/// This can be used to inspect delegates, or combine them via Sigil.
/// </summary>
/// <typeparam name="TDelegateType">The type of delegate being disassembled</typeparam>
public sealed class Disassembler<TDelegateType>
    where TDelegateType : class
{
    private sealed class LabelTracker
    {
        private HashSet<int> _markAt = new HashSet<int>();
        public IEnumerable<int> MarkAt { get { return _markAt; } }

        public void Mark(int at)
        {
            _markAt.Add(at);
        }
    }

    private const byte ContinueOpcode = 0xFE;
    private static readonly Dictionary<int, OpCode> _oneByteOps;
    private static readonly Dictionary<int, OpCode> _twoByteOps;

    static Disassembler()
    {
        var oneByte = new List<OpCode>();
        var twoByte = new List<OpCode>();

        foreach(var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var op = (OpCode)field.GetValue(null);

            if (op.Size == 1)
            {
                oneByte.Add(op);
                continue;
            }

            if (op.Size == 2)
            {
                twoByte.Add(op);
                continue;
            }

            throw new Exception("Unexpected op size for " + op);
        }

        _oneByteOps = oneByte.ToDictionary(d => (int)d.Value, d => d);
        _twoByteOps = twoByte.ToDictionary(d => (int)(d.Value & 0xFF), d => d);
    }

    private static void CheckDelegateType()
    {
        var delType = typeof(TDelegateType);

        var baseTypes = new HashSet<Type>();
        baseTypes.Add(delType);
        var bType = delType.BaseType;
        while (bType != null)
        {
            baseTypes.Add(bType);
            bType = bType.BaseType;
        }

        if (!baseTypes.Contains(typeof(Delegate)))
        {
            throw new ArgumentException("DelegateType must be a delegate, found " + delType.FullName);
        }
    }

    /// <summary>
    /// Disassembles a delegate into a DisassembledOperations object.
    /// </summary>
    /// <param name="del">The delegate to disassemble</param>
    public static DisassembledOperations<TDelegateType> Disassemble(TDelegateType del)
    {
        CheckDelegateType();

        var asDel = (Delegate)(object)del;

        var method = asDel.Method;
        var body = method.GetMethodBody();

        var cil = body.GetILAsByteArray();
        var locals = body.LocalVariables;
        var @params = asDel.Method.GetParameters();
        var excBlocks = body.ExceptionHandlingClauses;

        var convertedParams = new List<ParameterInfo>(@params).Select(s => SigilParameter.For(s)).ToList();

        if (asDel.Target != null)
        {
            convertedParams.Insert(0, new SigilParameter(0, asDel.Target.GetType()));
        }

        var ps = convertedParams;

        var ls =
            new List<LocalVariableInfo>(locals)
                .OrderBy(_ => _.LocalIndex)
                .Select((l, ix) => new SigilLocal(null, (ushort)l.LocalIndex, l.LocalType, null, "_local"+ix, null, 0))
                .ToList();

        var labels = new LabelTracker();
        var asLabels = new List<SigilLabel>();
        var needsInference = new List<Tuple<int, Operation<TDelegateType>>>();
        var ops =
            new List<Tuple<int, Operation<TDelegateType>>>(
                GetOperations(asDel.Method.Module, cil, ps, ls, labels, excBlocks, asLabels, needsInference)
            );

        var markAt = new Dictionary<int, Tuple<int, string>>();
        foreach (var at in labels.MarkAt)
        {
            var ix = IndexOfOpLastAt(ops, at);
            var name = "_label" + at;
            markAt[ix] = Tuple.Create(at, name);
        }

        foreach (var k in markAt.Keys.OrderByDescending(static k => k))
        {
            var pair = markAt[k];
            var name = asLabels.Where(l => l.Name == pair.Item2).Single();
            var ix = pair.Item1;

            var mark =
                new Operation<TDelegateType>
                {
                    IsMarkLabel = true,
                    LabelName = name.Name,
                    Parameters = new object[] { name },
                    Replay = emit => emit.MarkLabel(name),
                };

            ops.Insert(k, Tuple.Create(ix, mark));
        }

        ops = OrderOperations(ops);

        var prevCount = needsInference.Count;
        while (needsInference.Count > 0)
        {
            ops = InferTypes(ops, needsInference, ps, ls, asLabels);

            needsInference = new List<Tuple<int, Operation<TDelegateType>>>(ops.Where(w => w.Item2.IsOpCode && w.Item2.Parameters == null));

            if (prevCount == needsInference.Count)
            {
                throw new Exception("Disassembler was unable to infer necessary types");
            }

            prevCount = needsInference.Count;
        }


        var hasThis = (method.CallingConvention | CallingConventions.HasThis) != 0;
        bool usesThis;
        if (hasThis)
        {
            usesThis = false;
            // check to see if we actually use `this`, and if so we can't actually emit
            foreach (var op in ops)
            {
                if(op.Item2.IsOpCode)
                {
                    var opcode = op.Item2.OpCode;

                    if(opcode == OpCodes.Ldarg_0)
                    {
                        usesThis = true;
                        break;
                    }

                    var pointsToIntArg =
                        opcode == OpCodes.Ldarg ||
                        opcode == OpCodes.Ldarga ||
                        opcode == OpCodes.Starg;

                    if (pointsToIntArg)
                    {
                        var param = (int)op.Item2.Parameters.ElementAt(0);

                        if (param == 0)
                        {
                            usesThis = true;
                            break;
                        }
                    }

                    var pointsToShortArg =
                        opcode == OpCodes.Ldarg_S ||
                        opcode == OpCodes.Ldarga_S ||
                        opcode == OpCodes.Starg_S;

                    if (pointsToShortArg)
                    {
                        var param = (ushort)op.Item2.Parameters.ElementAt(0);

                        if (param == 0)
                        {
                            usesThis = true;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            usesThis = false;
        }

        var canBeEmitted = asDel.Target == null || !usesThis;

        return
            new DisassembledOperations<TDelegateType>(
                new List<Operation<TDelegateType>>(new List<Tuple<int, Operation<TDelegateType>>>(ops).Select(d => d.Item2)),
                ps,
                ls,
                asLabels,
                canBeEmitted
            );
    }

    private static List<Tuple<int, Operation<TDelegateType>>> InferTypes(
        List<Tuple<int, Operation<TDelegateType>>> ops,
        List<Tuple<int, Operation<TDelegateType>>> infer,
        IEnumerable<SigilParameter> ps,
        IEnumerable<SigilLocal> ls,
        IEnumerable<SigilLabel> asLabels)
    {
        var tempDisasm = new DisassembledOperations<TDelegateType>(
            new List<Operation<TDelegateType>>(new List<Tuple<int, Operation<TDelegateType>>>(ops).Select(d => d.Item2)),
            ps,
            ls,
            asLabels,
            canEmit: false
        );

        var use = new List<OperationResultUsage<TDelegateType>>(tempDisasm.Usage);

        ls.ForEach(l => l.SetOwner(null));
        asLabels.ForEach(l => l.SetOwner(null));

        var inferOps = infer.Select(s => s.Item2.OpCode).Distinct().ToList();

        var traced = use.Where(u => inferOps.Contains(u.ProducesResult.OpCode)).ToList();

        foreach (var t in traced)
        {
            var toReplace = infer[0];

            var couldBe = use.Where(u => u.ResultUsedBy.Contains(t.ProducesResult)).ToList();
            var couldBeTypes = couldBe.SelectMany(c => c.TypesProduced).Distinct().ToList();

            var op = MakeInferredReplayableOp(t.ProducesResult.OpCode, couldBeTypes, toReplace.Item2.Prefixes);

            infer.RemoveAt(0);

            if(op != null)
            {
                var replaceAt = ops.IndexOf(toReplace);
                ops[replaceAt] = Tuple.Create(toReplace.Item1, op);
            }
        }

        return ops;
    }

    private static Operation<TDelegateType> MakeInferredReplayableOp(OpCode op, List<TypeOnStack> consumesType, PrefixTracker prefixes)
    {
        if (consumesType.Any(c => c.Type == typeof(WildcardType))) return null;

        if (op == OpCodes.Ldlen)
        {
            var elem = consumesType.Single().Type.GetElementType();

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { elem },
                    Replay = emit => emit.LoadLength(elem),
                };
        }

        if (op == OpCodes.Ldelem_Ref)
        {
            var elem = consumesType.Where(x => x.IsArray).Single().Type.GetElementType();

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { elem },
                    Replay = emit => emit.LoadElement(elem),
                };
        }

        if (op == OpCodes.Stelem_Ref)
        {
            var vals = consumesType.Where(x => !x.Type.IsValueType).ToList();
            var arr = vals.OrderByDescending(v => v.IsArray ? v.Type.GetArrayRank() : 0).First();

            var elem = arr.Type.GetElementType();

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { elem },
                    Replay = emit => emit.StoreElement(elem),
                };
        }

        if (op == OpCodes.Ldind_Ref)
        {
            var elem = consumesType.Single().Type.GetElementType();
            var isVolatile = prefixes.HasVolatile;
            int? unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { elem, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(elem, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_Ref)
        {
            var arr = consumesType.OrderByDescending(v => v.IsArray ? v.Type.GetArrayRank() : 0).First();
            var elem = arr.Type.GetElementType();
            var isVolatile = prefixes.HasVolatile;
            int? unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { elem, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(elem, isVolatile, unaligned),
                };
        }

        throw new Exception("Encountered unexpected operation [" + op + "] which requires type inferencing");
    }

    private static List<Tuple<int, Operation<TDelegateType>>> OrderOperations(List<Tuple<int, Operation<TDelegateType>>> ops)
    {
        var ret = new List<Tuple<int, Operation<TDelegateType>>>();

        var grouped = ops.GroupBy(g => g.Item1);

        foreach (var group in grouped.OrderBy(o => o.Key))
        {
            var inOrder = group.Select(x => x.Item2)
                .OrderBy<Operation<TDelegateType>, int>(
                    op =>
                    {
                        if (op.IsCatchBlockEnd) return -1000000;
                        if (op.IsFinallyBlockEnd) return -10000;
                        if (op.IsExceptionBlockEnd) return -1000;

                        if (op.IsExceptionBlockStart) return -100;
                        if (op.IsCatchBlockStart) return -10;
                        if (op.IsFinallyBlockStart) return -1;

                        if (op.IsMarkLabel) return 0;

                        return 1;
                    }
                );

            foreach (var i in inOrder)
            {
                ret.Add(Tuple.Create(group.Key, i));
            }
        }

        return ret;
    }

    private static int IndexOfOpLastAt(IEnumerable<Tuple<int, Operation<TDelegateType>>> ops, int ix)
    {
        bool foundIt = false;
        var i = 0;
        foreach (var x in ops)
        {
            if (x.Item1 == ix)
            {
                foundIt = true;
            }

            if (foundIt && x.Item1 != ix)
            {
                return i - 1;
            }

            i++;
        }

        if (foundIt)
        {
            return i - 1;
        }

        throw new Exception("Couldn't find label position in operations for " + ix);
    }

    private static int DeclarationNumber(Dictionary<int, List<ExceptionHandlingClause>> start, ExceptionHandlingClause exc)
    {
        var order = start.Keys.Select(k => k).OrderBy(k => k).ToList();

        int ct = 0;

        foreach (var k in order)
        {
            var set = start[k];
            var ix = set.IndexOf(exc);

            if (ix == -1)
            {
                ct += set.Count;
            }
            else
            {
                ct += ix;
                return ct;
            }
        }

        throw new Exception("Couldn't find [" + exc + "] in starting exception blocks");
    }

    private static IEnumerable<Tuple<int, Operation<TDelegateType>>> GetOperations(
        Module mod,
        byte[] cil,
        IEnumerable<SigilParameter> ps,
        IEnumerable<SigilLocal> ls,
        LabelTracker labels,
        IList<ExceptionHandlingClause> exceptions,
        List<SigilLabel> labelAccumulator,
        List<Tuple<int, Operation<TDelegateType>>> needsInference)
    {
        var exceptionStart = new Dictionary<int, List<ExceptionHandlingClause>>();
        var exceptionEnd = new Dictionary<int, List<ExceptionHandlingClause>>();
        var catchStart = new Dictionary<int, List<ExceptionHandlingClause>>();
        var catchEnd = new Dictionary<int, List<ExceptionHandlingClause>>();
        var finallyStart = new Dictionary<int, List<ExceptionHandlingClause>>();
        var finallyEnd = new Dictionary<int, List<ExceptionHandlingClause>>();

        var activeExceptionBlocks = new Dictionary<ExceptionHandlingClause, string>();
        var activeCatchBlocks = new Dictionary<ExceptionHandlingClause, string>();
        var activeFinallyBlocks = new Dictionary<ExceptionHandlingClause, string>();

        foreach (var exc in exceptions)
        {
            List<ExceptionHandlingClause> eStart, eEnd;

            if (!exceptionStart.TryGetValue(exc.TryOffset, out eStart))
            {
                exceptionStart[exc.TryOffset] = eStart = new List<ExceptionHandlingClause>();
            }

            if(!exceptionEnd.TryGetValue(exc.HandlerOffset + exc.HandlerLength, out eEnd))
            {
                exceptionEnd[exc.HandlerOffset + exc.HandlerLength] = eEnd = new List<ExceptionHandlingClause>();
            }

            eStart.Add(exc);
            eEnd.Add(exc);

            if (exc.Flags == ExceptionHandlingClauseOptions.Clause)
            {
                List<ExceptionHandlingClause> cStart, cEnd;

                if (!catchStart.TryGetValue(exc.HandlerOffset, out cStart))
                {
                    catchStart[exc.HandlerOffset] = cStart = new List<ExceptionHandlingClause>();
                }

                if (!catchEnd.TryGetValue(exc.HandlerOffset + exc.HandlerLength, out cEnd))
                {
                    catchEnd[exc.HandlerOffset + exc.HandlerLength] = cEnd = new List<ExceptionHandlingClause>();
                }

                cStart.Add(exc);
                cEnd.Add(exc);

                continue;
            }

            if (exc.Flags == ExceptionHandlingClauseOptions.Finally)
            {
                List<ExceptionHandlingClause> fStart, fEnd;

                if (!finallyStart.TryGetValue(exc.HandlerOffset, out fStart))
                {
                    finallyStart[exc.HandlerOffset] = fStart = new List<ExceptionHandlingClause>();
                }

                if (!finallyEnd.TryGetValue(exc.HandlerOffset + exc.HandlerLength, out fEnd))
                {
                    finallyEnd[exc.HandlerOffset + exc.HandlerLength] = fEnd = new List<ExceptionHandlingClause>();
                }

                fStart.Add(exc);
                fEnd.Add(exc);

                continue;
            }

            throw new InvalidOperationException("Unexpected exception handling clause, Sigil only supports try/catch/finally.");
        }

        foreach (var k in (new List<int>(exceptionStart.Keys)))
        {
            exceptionStart[k] = exceptionStart[k].OrderByDescending(x => x.TryLength + x.HandlerLength).ToList();
        }

        foreach (var k in (new List<int>(exceptionEnd.Keys)))
        {
            exceptionEnd[k] = exceptionEnd[k].OrderBy(x => x.TryLength + x.HandlerLength).ToList();
        }

        foreach (var k in (new List<int>(catchStart.Keys)))
        {
            catchStart[k] = catchStart[k].OrderBy(x => x.TryLength + x.HandlerLength).ToList();
        }

        foreach (var k in (new List<int>(catchEnd.Keys)))
        {
            catchEnd[k] = catchEnd[k].OrderBy(x => x.TryLength + x.HandlerLength).ToList();
        }

        foreach (var k in (new List<int>(finallyStart.Keys)))
        {
            finallyStart[k] = finallyStart[k].OrderBy(x => x.TryLength + x.HandlerLength).ToList();
        }

        foreach (var k in (new List<int>(finallyEnd.Keys)))
        {
            finallyEnd[k] = finallyEnd[k].OrderBy(x => x.TryLength + x.HandlerLength).ToList();
        }

        var parameterLookup = new Dictionary<int, SigilParameter>();
        var localLookup = new Dictionary<int, SigilLocal>();

        foreach (var p in ps)
        {
            parameterLookup[p.Position] = p;
        }

        foreach (var l in ls)
        {
            localLookup[l.Index] = l;
        }

        var ret = new List<Tuple<int, Operation<TDelegateType>>>();
        var prefixes = new PrefixTracker();

        CheckForExceptionOperations(0, exceptionStart, exceptionEnd, catchStart, catchEnd, finallyStart, finallyEnd, activeExceptionBlocks, activeCatchBlocks, activeFinallyBlocks, ret);

        int? gap = null;
        int i = 0;
        while (i < cil.Length)
        {
            var startsAt = i;
            Operation<TDelegateType> op;
            i += ReadOp(mod, cil, i, parameterLookup, localLookup, prefixes, labels, labelAccumulator, out op);

            if (op != null)
            {
                if (!op.IsIgnored)
                {
                    var toAdd = Tuple.Create(gap ?? startsAt, op);

                    ret.Add(toAdd);

                    if (op.Parameters == null)
                    {
                        needsInference.Add(toAdd);
                    }
                }

                prefixes.Clear();
                gap = null;
            }
            else
            {
                gap = gap ?? startsAt;
            }

            CheckForExceptionOperations(i, exceptionStart, exceptionEnd, catchStart, catchEnd, finallyStart, finallyEnd, activeExceptionBlocks, activeCatchBlocks, activeFinallyBlocks, ret);
        }

        return ret;
    }

    private static void CheckForExceptionOperations(
        int i,
        Dictionary<int, List<ExceptionHandlingClause>> exceptionStart,
        Dictionary<int, List<ExceptionHandlingClause>> exceptionEnd,
        Dictionary<int, List<ExceptionHandlingClause>> catchStart,
        Dictionary<int, List<ExceptionHandlingClause>> catchEnd,
        Dictionary<int, List<ExceptionHandlingClause>> finallyStart,
        Dictionary<int, List<ExceptionHandlingClause>> finallyEnd,
        Dictionary<ExceptionHandlingClause, string> activeExceptionBlocks,
        Dictionary<ExceptionHandlingClause, string> activeCatchBlocks,
        Dictionary<ExceptionHandlingClause, string> activeFinallyBlocks,
        List<Tuple<int, Operation<TDelegateType>>> ret)
    {
        if (catchEnd.ContainsKey(i))
        {
            foreach (var exc in catchEnd[i])
            {
                var c = activeCatchBlocks[exc];

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsCatchBlockEnd = true,
                            Parameters = new object[0],
                            Replay = emit => emit.EndCatchBlock(c),
                        }
                    )
                );

                activeCatchBlocks.Remove(exc);
            }
        }

        if (finallyEnd.ContainsKey(i))
        {
            foreach (var exc in finallyEnd[i])
            {
                var f = activeFinallyBlocks[exc];

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsFinallyBlockEnd = true,
                            Parameters = new object[0],
                            Replay = emit => emit.EndFinallyBlock(f),
                        }
                    )
                );

                activeFinallyBlocks.Remove(exc);
            }
        }

        // Must be checked last as catch & finally must end before the exception block overall
        if (exceptionEnd.ContainsKey(i))
        {
            foreach (var exc in exceptionEnd[i])
            {
                var name = activeExceptionBlocks[exc];

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsExceptionBlockEnd = true,
                            Parameters = new object[0],
                            Replay = emit => emit.EndExceptionBlock(name),
                        }
                    )
                );

                activeExceptionBlocks.Remove(exc);
            }
        }

        if (exceptionStart.ContainsKey(i))
        {
            foreach (var exc in exceptionStart[i])
            {
                var name = "__exc-" + Guid.NewGuid();

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsExceptionBlockStart = true,
                            Parameters = new object[0],
                            Replay = emit => emit.BeginExceptionBlock(name),
                        }
                    )
                );

                activeExceptionBlocks[exc] = name;
            }
        }

        if (catchStart.ContainsKey(i))
        {
            foreach (var exc in catchStart[i])
            {
                var name = activeExceptionBlocks[exc];

                var catchName = "__catch-" + Guid.NewGuid();

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsCatchBlockStart = true,
                            Parameters = new object[0],
                            Replay = emit => emit.BeginCatchBlock(name, exc.CatchType, catchName),
                        }
                    )
                );

                activeCatchBlocks[exc] = catchName;
            }
        }

        if (finallyStart.ContainsKey(i))
        {
            foreach (var exc in finallyStart[i])
            {
                var name = activeExceptionBlocks[exc];

                var finallyName = "__finally-" + Guid.NewGuid();

                ret.Add(
                    Tuple.Create(
                        i,
                        new Operation<TDelegateType>
                        {
                            IsFinallyBlockStart = true,
                            Parameters = new object[0],
                            Replay = emit => emit.BeginFinallyBlock(name, finallyName),
                        }
                    )
                );

                activeFinallyBlocks[exc] = finallyName;
            }
        }
    }

    private static int ReadOp(
        Module mod,
        byte[] cil,
        int ix,
        IDictionary<int, SigilParameter> pLookup,
        IDictionary<int, SigilLocal> lLookup,
        PrefixTracker prefixes,
        LabelTracker labels,
        List<SigilLabel> labelAccumulator,
        out Operation<TDelegateType> op)
    {
        int advance = 0;

        OpCode opcode;
        byte first = cil[ix];

        if (first == ContinueOpcode)
        {
            var next = cil[ix + 1];

            opcode = _twoByteOps[next];
            advance += 2;
        }
        else
        {
            opcode = _oneByteOps[first];
            advance++;
        }

        var operand = ReadOperands(mod, opcode, cil, ix, ix + advance, pLookup, lLookup, ref advance);

        op = MakeReplayableOperation(opcode, operand, prefixes, labels, labelAccumulator, lLookup);

        return advance;
    }

    private static SigilLabel ChooseLabel(int absAddr, LabelTracker labels, List<SigilLabel> labelAccumulator)
    {
        var name = "_label" + absAddr;

        var ret = labelAccumulator.Where(l => l.Name == name).SingleOrDefault();

        if (ret == null)
        {
            ret = new SigilLabel(null, null, name);
            labelAccumulator.Add(ret);
        }

        labels.Mark(absAddr);
        return ret;
    }

    private static Operation<TDelegateType> MakeReplayableOperation(
        OpCode op,
        object[] operands,
        PrefixTracker prefixes,
        LabelTracker labels,
        List<SigilLabel> labelAccumulator,
        IDictionary<int, SigilLocal> locals)
    {
        if (op == OpCodes.Add)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Add(),
                };
        }

        if (op == OpCodes.Add_Ovf)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.AddOverflow(),
                };
        }

        if (op == OpCodes.Add_Ovf_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedAddOverflow(),
                };
        }

        if (op == OpCodes.And)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.And(),
                };
        }

        if (op == OpCodes.Arglist)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ArgumentList(),
                };
        }

        if (op == OpCodes.Beq || op == OpCodes.Beq_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfEqual(label),
                };
        }

        if (op == OpCodes.Bge || op == OpCodes.Bge_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfGreaterOrEqual(label),
                };
        }

        if (op == OpCodes.Bge_Un || op == OpCodes.Bge_Un_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.UnsignedBranchIfGreaterOrEqual(label),
                };
        }

        if (op == OpCodes.Bgt || op == OpCodes.Bgt_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfGreater(label),
                };
        }

        if (op == OpCodes.Bgt_Un || op == OpCodes.Bgt_Un_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.UnsignedBranchIfGreater(label),
                };
        }

        if (op == OpCodes.Ble || op == OpCodes.Ble_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfLessOrEqual(label),
                };
        }

        if (op == OpCodes.Ble_Un || op == OpCodes.Ble_Un_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.UnsignedBranchIfLessOrEqual(label),
                };
        }

        if (op == OpCodes.Blt || op == OpCodes.Blt_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfLess(label),
                };
        }

        if (op == OpCodes.Blt_Un || op == OpCodes.Blt_Un_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.UnsignedBranchIfLess(label),
                };
        }

        if (op == OpCodes.Bne_Un || op == OpCodes.Bne_Un_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.UnsignedBranchIfNotEqual(label),
                };
        }

        if (op == OpCodes.Box)
        {
            var valType = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { valType },
                    Replay = emit => emit.Box(valType),
                };
        }

        if (op == OpCodes.Br || op == OpCodes.Br_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.Branch(label),
                };
        }

        if (op == OpCodes.Break)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Break(),
                };
        }

        if (op == OpCodes.Brfalse || op == OpCodes.Brfalse_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfFalse(label),
                };
        }

        if (op == OpCodes.Brtrue || op == OpCodes.Brtrue_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.BranchIfTrue(label),
                };
        }

        if (op == OpCodes.Call)
        {
            var mem = (MemberInfo)operands[0];
            var mtd = (MethodInfo)mem;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { mtd },
                    Replay = emit => emit.Call(mtd, null),
                };
        }

        if (op == OpCodes.Calli)
        {
            throw new NotImplementedException("Calli is not supported in Sigil.Disassembler");
        }

        if (op == OpCodes.Callvirt)
        {
            var mem = (MemberInfo)operands[0];
            var mtd = (MethodInfo)mem;
            var cnstd = prefixes.HasConstrained ? prefixes.Constrained : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { mtd, cnstd, null },
                    Replay = emit => emit.CallVirtual(mtd, cnstd, null),
                };
        }

        if (op == OpCodes.Castclass)
        {
            var type = (Type)operands[0];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.CastClass(type),
                };
        }

        if (op == OpCodes.Ceq)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.CompareEqual(),
                };
        }

        if (op == OpCodes.Cgt)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.CompareGreaterThan(),
                };
        }

        if (op == OpCodes.Cgt_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedCompareGreaterThan(),
                };
        }

        if (op == OpCodes.Ckfinite)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.CheckFinite(),
                };
        }

        if (op == OpCodes.Clt)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.CompareLessThan(),
                };
        }

        if (op == OpCodes.Clt_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedCompareLessThan(),
                };
        }

        if (op == OpCodes.Constrained)
        {
            var type = (Type)operands[0];
            prefixes.SetConstrained(type);

            return null;
        }

        if (op == OpCodes.Conv_I)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(IntPtr)),
                };
        }

        if (op == OpCodes.Conv_I1)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(sbyte)),
                };
        }

        if (op == OpCodes.Conv_I2)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(short)),
                };
        }

        if (op == OpCodes.Conv_I4)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(int)),
                };
        }

        if (op == OpCodes.Conv_I8)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(long)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(IntPtr)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(IntPtr)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I1)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(sbyte)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I1_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(sbyte)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I2)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(short)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I2_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(short)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I4)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(int)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I4_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(int)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I8)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(long)),
                };
        }

        if (op == OpCodes.Conv_Ovf_I8_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(long)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(UIntPtr)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(UIntPtr)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U1)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(byte)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U1_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(byte)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U2)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(ushort)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U2_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(ushort)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U4)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(uint)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U4_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(uint)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U8)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ConvertOverflow(typeof(ulong)),
                };
        }

        if (op == OpCodes.Conv_Ovf_U8_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertOverflow(typeof(ulong)),
                };
        }

        if (op == OpCodes.Conv_R_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedConvertToFloat(),
                };
        }

        if (op == OpCodes.Conv_R4)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(float)),
                };
        }

        if (op == OpCodes.Conv_R8)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(double)),
                };
        }

        if (op == OpCodes.Conv_U)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(UIntPtr)),
                };
        }

        if (op == OpCodes.Conv_U1)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(byte)),
                };
        }

        if (op == OpCodes.Conv_U2)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(ushort)),
                };
        }

        if (op == OpCodes.Conv_U4)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(uint)),
                };
        }

        if (op == OpCodes.Conv_U8)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Convert(typeof(ulong)),
                };
        }

        if (op == OpCodes.Cpblk)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.CopyBlock(false, null),
                };
        }

        if (op == OpCodes.Cpobj)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.CopyObject(type),
                };
        }

        if (op == OpCodes.Div)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Divide(),
                };
        }

        if (op == OpCodes.Div_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedDivide(),
                };
        }

        if (op == OpCodes.Dup)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Duplicate(),
                };
        }

        if (op == OpCodes.Endfilter)
        {
            throw new InvalidOperationException("Sigil does not support fault blocks, or the Endfilter opcode");
        }

        if (op == OpCodes.Endfinally)
        {
            // Endfinally isn't emitted directly by ILGenerator or Sigil; it's implicit in EndFinallyBlock() calls
            return
                new Operation<TDelegateType>
                {
                    IsIgnored = true,
                };
        }

        if (op == OpCodes.Initblk)
        {
            var isVolatile = prefixes.HasVolatile;
            int? unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { isVolatile, unaligned },
                    Replay = emit => emit.InitializeBlock(isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Initobj)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.InitializeObject(type),
                };
        }

        if (op == OpCodes.Isinst)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.IsInstance(type),
                };
        }

        if (op == OpCodes.Jmp)
        {
            var mtd = (MethodInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { mtd },
                    Replay = emit => emit.Jump(mtd),
                };
        }

        if (op == OpCodes.Ldarg)
        {
            ushort ix = (ushort)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarg_0)
        {
            ushort ix = 0;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarg_1)
        {
            ushort ix = 1;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarg_2)
        {
            ushort ix = 2;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarg_3)
        {
            ushort ix = 3;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarg_S)
        {
            ushort ix = (byte)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgument(ix),
                };
        }

        if (op == OpCodes.Ldarga)
        {
            ushort ix = (ushort)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgumentAddress(ix),
                };
        }

        if (op == OpCodes.Ldarga_S)
        {
            ushort ix = (byte)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.LoadArgumentAddress(ix),
                };
        }

        if (op == OpCodes.Ldc_I4)
        {
            int c = (int)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_0)
        {
            int c = 0;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_1)
        {
            int c = 1;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_2)
        {
            int c = 2;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_3)
        {
            int c = 3;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_4)
        {
            int c = 4;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_5)
        {
            int c = 5;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_6)
        {
            int c = 6;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_7)
        {
            int c = 7;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_8)
        {
            int c = 8;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_M1)
        {
            int c = -1;
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I4_S)
        {
            int c = (sbyte)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_I8)
        {
            long c = (long)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_R4)
        {
            float c = (float)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldc_R8)
        {
            double c = (double)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { c },
                    Replay = emit => emit.LoadConstant(c),
                };
        }

        if (op == OpCodes.Ldelem)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_I)
        {
            var type = typeof(IntPtr);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_I1)
        {
            var type = typeof(sbyte);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_I2)
        {
            var type = typeof(short);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_I4)
        {
            var type = typeof(int);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_I8)
        {
            var type = typeof(long);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_R4)
        {
            var type = typeof(float);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_R8)
        {
            var type = typeof(double);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_Ref)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = null,
                    Replay = emit => emit.LoadElement(typeof(WildcardType)),
                };
        }

        if (op == OpCodes.Ldelem_U1)
        {
            var type = typeof(byte);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_U2)
        {
            var type = typeof(ushort);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelem_U4)
        {
            var type = typeof(uint);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElement(type),
                };
        }

        if (op == OpCodes.Ldelema)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.LoadElementAddress(type),
                };
        }

        if (op == OpCodes.Ldfld)
        {
            var fld = (FieldInfo)operands[0];
            var isVolatile = prefixes.HasVolatile;
            int? unalgined = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld, isVolatile, unalgined },
                    Replay = emit => emit.LoadField(fld, isVolatile, unalgined),
                };
        }

        if (op == OpCodes.Ldflda)
        {
            var fld = (FieldInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld },
                    Replay = emit => emit.LoadFieldAddress(fld),
                };
        }

        if (op == OpCodes.Ldftn)
        {
            var mtd = (MethodInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { mtd },
                    Replay = emit => emit.LoadFunctionPointer(mtd),
                };
        }

        if (op == OpCodes.Ldind_I)
        {
            var type = typeof(IntPtr);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_I1)
        {
            var type = typeof(sbyte);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_I2)
        {
            var type = typeof(short);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_I4)
        {
            var type = typeof(int);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_I8)
        {
            var type = typeof(long);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_R4)
        {
            var type = typeof(float);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_R8)
        {
            var type = typeof(double);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_Ref)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = null,
                    Replay = emit => emit.LoadIndirect<WildcardType>(),

                    Prefixes = prefixes.Clone(),
                };
        }

        if (op == OpCodes.Ldind_U1)
        {
            var type = typeof(byte);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_U2)
        {
            var type = typeof(ushort);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldind_U4)
        {
            var type = typeof(uint);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldlen)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = null,
                    Replay = emit => emit.LoadLength<WildcardType>(),
                };
        }

        if (op == OpCodes.Ldloc)
        {
            ushort ix = (ushort)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloc_0)
        {
            ushort ix = 0;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloc_1)
        {
            ushort ix = 1;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloc_2)
        {
            ushort ix = 2;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloc_3)
        {
            ushort ix = 3;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloc_S)
        {
            ushort ix = (byte)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocal(loc),
                };
        }

        if (op == OpCodes.Ldloca)
        {
            ushort ix = (ushort)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocalAddress(loc),
                };
        }

        if (op == OpCodes.Ldloca_S)
        {
            ushort ix = (byte)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.LoadLocalAddress(loc),
                };
        }

        if (op == OpCodes.Ldnull)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.LoadNull(),
                };
        }

        if (op == OpCodes.Ldobj)
        {
            var type = (Type)operands[0];
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.LoadObject(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldsfld)
        {
            var fld = (FieldInfo)operands[0];
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld, isVolatile, unaligned },
                    Replay = emit => emit.LoadField(fld, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Ldsflda)
        {
            var fld = (FieldInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld },
                    Replay = emit => emit.LoadFieldAddress(fld),
                };
        }

        if (op == OpCodes.Ldstr)
        {
            var str = (string)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { str },
                    Replay = emit => emit.LoadConstant(str),
                };
        }

        if (op == OpCodes.Ldtoken)
        {
            var asFld = operands[0] as FieldInfo;
            var asMtd = operands[0] as MethodInfo;
            var asCtor = operands[0] as ConstructorInfo;
            var asType = operands[0] as Type;

            if (asFld != null)
            {
                return
                    new Operation<TDelegateType>
                    {
                        OpCode = op,
                        Parameters = new object[] { asFld },
                        Replay = emit => emit.LoadConstant(asFld),
                    };
            }

            if (asMtd != null)
            {
                return
                    new Operation<TDelegateType>
                    {
                        OpCode = op,
                        Parameters = new object[] { asMtd },
                        Replay = emit => emit.LoadConstant(asMtd),
                    };
            }

            if(asCtor != null)
            {
                return
                    new Operation<TDelegateType>
                    {
                        OpCode = op,
                        Parameters = new object[] { asCtor },
                        Replay = emit => emit.LoadConstant(asCtor),
                    };
            }

            if (asType != null)
            {
                return
                    new Operation<TDelegateType>
                    {
                        OpCode = op,
                        Parameters = new object[] { asType },
                        Replay = emit => emit.LoadConstant(asType),
                    };
            }

            throw new Exception("Unexpected operand for ldtoken [" + operands[0] + "]");
        }

        if (op == OpCodes.Ldvirtftn)
        {
            var mtd = (MethodInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { mtd },
                    Replay = emit => emit.LoadVirtualFunctionPointer(mtd),
                };
        }

        if (op == OpCodes.Leave || op == OpCodes.Leave_S)
        {
            var absAddr = (int)operands[0];
            var label = ChooseLabel(absAddr, labels, labelAccumulator);

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { label },
                    Replay = emit => emit.Leave(label),
                };
        }

        if (op == OpCodes.Localloc)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.LocalAllocate(),
                };
        }

        if (op == OpCodes.Mkrefany)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.MakeReferenceAny(type),
                };
        }

        if (op == OpCodes.Mul)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Multiply(),
                };
        }

        if (op == OpCodes.Mul_Ovf)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.MultiplyOverflow(),
                };
        }

        if (op == OpCodes.Mul_Ovf_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedMultiplyOverflow(),
                };
        }

        if (op == OpCodes.Neg)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Negate(),
                };
        }

        if (op == OpCodes.Newarr)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.NewArray(type),
                };
        }

        if (op == OpCodes.Newobj)
        {
            var ctor = (ConstructorInfo)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ctor },
                    Replay = emit => emit.NewObject(ctor),
                };
        }

        if (op == OpCodes.Nop)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Nop(),
                };
        }

        if (op == OpCodes.Not)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Not(),
                };
        }

        if (op == OpCodes.Or)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Or(),
                };
        }

        if (op == OpCodes.Pop)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Pop(),
                };
        }

        if (op == OpCodes.Prefix1 || op == OpCodes.Prefix2 || op == OpCodes.Prefix3 || op == OpCodes.Prefix4 || op == OpCodes.Prefix5 || op == OpCodes.Prefix6 || op == OpCodes.Prefix7 || op == OpCodes.Prefixref)
        {
            throw new InvalidOperationException("Encountered reserved opcode [" + op + "]");
        }

        if (op == OpCodes.Readonly)
        {
            prefixes.SetReadOnly();
            return null;
        }

        if (op == OpCodes.Refanytype)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ReferenceAnyType(),
                };
        }

        if (op == OpCodes.Refanyval)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ReferenceAnyValue(type),
                };
        }

        if (op == OpCodes.Rem)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Remainder(),
                };
        }

        if (op == OpCodes.Rem_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedRemainder(),
                };
        }

        if (op == OpCodes.Ret)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Return(),
                };
        }

        if (op == OpCodes.Rethrow)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ReThrow(),
                };
        }

        if (op == OpCodes.Shl)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ShiftLeft(),
                };
        }

        if (op == OpCodes.Shr)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.ShiftRight(),
                };
        }

        if (op == OpCodes.Shr_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedShiftRight(),
                };
        }

        if (op == OpCodes.Sizeof)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.SizeOf(type),
                };
        }

        if (op == OpCodes.Starg)
        {
            ushort ix = (ushort)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.StoreArgument(ix),
                };
        }

        if (op == OpCodes.Starg_S)
        {
            ushort ix = (byte)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { ix },
                    Replay = emit => emit.StoreArgument(ix),
                };
        }

        if (op == OpCodes.Stelem)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_I)
        {
            var type = typeof(IntPtr);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_I1)
        {
            var type = typeof(sbyte);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_I2)
        {
            var type = typeof(short);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_I4)
        {
            var type = typeof(int);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_I8)
        {
            var type = typeof(long);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_R4)
        {
            var type = typeof(float);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_R8)
        {
            var type = typeof(double);
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.StoreElement(type),
                };
        }

        if (op == OpCodes.Stelem_Ref)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = null,
                    Replay = emit => emit.StoreElement<WildcardType>(),
                };
        }

        if (op == OpCodes.Stfld)
        {
            var fld = (FieldInfo)operands[0];
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld, isVolatile, unaligned },
                    Replay = emit => emit.StoreField(fld, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_I)
        {
            var type = typeof(IntPtr);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_I1)
        {
            var type = typeof(sbyte);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_I2)
        {
            var type = typeof(short);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_I4)
        {
            var type = typeof(int);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_I8)
        {
            var type = typeof(long);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_R4)
        {
            var type = typeof(float);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_R8)
        {
            var type = typeof(double);
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreIndirect(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stind_Ref)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = null,
                    Replay = emit => emit.StoreIndirect<WildcardType>(),

                    Prefixes = prefixes.Clone(),
                };
        }

        if (op == OpCodes.Stloc)
        {
            ushort ix = (ushort)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stloc_0)
        {
            ushort ix = 0;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stloc_1)
        {
            ushort ix = 1;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stloc_2)
        {
            ushort ix = 2;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stloc_3)
        {
            ushort ix = 3;
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stloc_S)
        {
            ushort ix = (byte)operands[0];
            var loc = locals[ix];

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { loc },
                    Replay = emit => emit.StoreLocal(loc),
                };
        }

        if (op == OpCodes.Stobj)
        {
            var type = (Type)operands[0];
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type, isVolatile, unaligned },
                    Replay = emit => emit.StoreObject(type, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Stsfld)
        {
            var fld = (FieldInfo)operands[0];
            var isVolatile = prefixes.HasVolatile;
            var unaligned = prefixes.HasUnaligned ? prefixes.Unaligned : null;

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { fld, isVolatile, unaligned },
                    Replay = emit => emit.StoreField(fld, isVolatile, unaligned),
                };
        }

        if (op == OpCodes.Sub)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Subtract(),
                };
        }

        if (op == OpCodes.Sub_Ovf)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.SubtractOverflow(),
                };
        }

        if (op == OpCodes.Sub_Ovf_Un)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.UnsignedSubtractOverflow(),
                };
        }

        if (op == OpCodes.Switch)
        {
            var swLabls = new SigilLabel[operands.Length];

            for(var i = 0; i < operands.Length; i++)
            {
                var abs = (int)operands[i];
                var label = ChooseLabel(abs, labels, labelAccumulator);
                swLabls[i] = label;
            }

            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { swLabls },
                    Replay = emit => emit.Switch(swLabls),
                };
        }

        if (op == OpCodes.Tailcall)
        {
            prefixes.SetTailCall();
            return null;
        }

        if (op == OpCodes.Throw)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Throw(),
                };
        }

        if (op == OpCodes.Unaligned)
        {
            byte u = (byte)operands[0];
            prefixes.SetUnaligned(u);
            return null;
        }

        if (op == OpCodes.Unbox)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.Unbox(type),
                };
        }

        if (op == OpCodes.Unbox_Any)
        {
            var type = (Type)operands[0];
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[] { type },
                    Replay = emit => emit.UnboxAny(type),
                };
        }

        if (op == OpCodes.Volatile)
        {
            prefixes.SetVolatile();

            return null;
        }

        if (op == OpCodes.Xor)
        {
            return
                new Operation<TDelegateType>
                {
                    OpCode = op,
                    Parameters = new object[0],
                    Replay = emit => emit.Xor(),
                };
        }

        throw new Exception("Unexpected opcode [" + op + "]");
    }

    private static long ReadLong(byte[] cil, int at)
    {
        var a = (uint)(cil[at] | (cil[at + 1] << 8) | (cil[at + 2] << 16) | (cil[at + 3] << 24));
        var b = (uint)(cil[at+4] | (cil[at + 5] << 8) | (cil[at + 6] << 16) | (cil[at + 7] << 24));

        return (((long)b) << 32) | a;
    }

    private static int ReadInt(byte[] cil, int at)
    {
        return cil[at] | (cil[at + 1] << 8) | (cil[at + 2] << 16) | (cil[at + 3] << 24);
    }

    private static short ReadShort(byte[] cil, int at)
    {
        return (short)(cil[at] | cil[at + 1]);
    }

    private static float ReadFloat(byte[] cil, int at)
    {
        var arr = new byte[4];
        arr[0] = cil[at];
        arr[1] = cil[at + 1];
        arr[2] = cil[at + 2];
        arr[3] = cil[at + 3];

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(arr);
        }

        return BitConverter.ToSingle(arr, 0);
    }

    private static double ReadDouble(byte[] cil, int at)
    {
        var arr = new byte[8];
        arr[0] = cil[at];
        arr[1] = cil[at + 1];
        arr[2] = cil[at + 2];
        arr[3] = cil[at + 3];
        arr[4] = cil[at + 4];
        arr[5] = cil[at + 5];
        arr[6] = cil[at + 6];
        arr[7] = cil[at + 7];

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(arr);
        }

        return BitConverter.ToDouble(arr, 0);
    }

    private static float ReadSingle(byte[] cil, int at)
    {
        var arr = new byte[4];
        arr[0] = cil[at];
        arr[1] = cil[at + 1];
        arr[2] = cil[at + 2];
        arr[3] = cil[at + 3];

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(arr);
        }

        return BitConverter.ToSingle(arr, 0);
    }

    private static object[] ReadOperands(Module mod, OpCode op, byte[] cil, int instrStart, int operandStart, IDictionary<int, SigilParameter> pLookup, IDictionary<int, SigilLocal> lLookup, ref int advance)
    {
        switch (op.OperandType)
        {
            case OperandType.InlineBrTarget:
                advance += 4;
                var offset = ReadInt(cil, operandStart);
                var jumpTarget = instrStart + advance + offset;
                return new object[] { jumpTarget };

            case OperandType.InlineSwitch:
                advance += 4;
                var len = ReadInt(cil, operandStart);
                var offset1 = instrStart + len * 4;
                var ret = new object[len];
                for (var i = 0; i < len; i++)
                {
                    var step = ReadInt(cil, operandStart + advance);
                    advance += 4;
                    ret[i] = offset1 + step;
                }
                return ret;

            case OperandType.ShortInlineBrTarget:
                advance += 1;
                var offset2 = (sbyte)cil[operandStart];
                var jumpTarget2 = instrStart + advance + offset2;
                return new object[] { jumpTarget2 };

            case OperandType.InlineField:
            case OperandType.InlineTok:
            case OperandType.InlineType:
            case OperandType.InlineMethod:
                advance += 4;
                var mem = mod.ResolveMember(ReadInt(cil, operandStart));
                return new object[] { mem };

            case OperandType.InlineI:
                advance += 4;
                return new object[] { ReadInt(cil, operandStart) };

            case OperandType.InlineI8:
                advance += 8;
                return new object[] { ReadLong(cil, operandStart) };

            case OperandType.InlineNone:
                return new object[0];

            case OperandType.InlineR:
                advance += 8;
                return new object[] { ReadDouble(cil, operandStart) };

            case OperandType.InlineSig:
                advance += 4;
                var sig = mod.ResolveSignature(ReadInt(cil, operandStart));
                return new object[] { sig };

            case OperandType.InlineString:
                advance += 4;
                var str = mod.ResolveString(ReadInt(cil, operandStart));
                return new object[] { str };

            case OperandType.InlineVar:
                advance += 2;
                return new object[] { ReadShort(cil, operandStart) };

            case OperandType.ShortInlineI:
                advance += 1;
                if (op == OpCodes.Ldc_I4_S)
                {
                    return new object[] { (sbyte)cil[operandStart] };
                }
                else
                {
                    return new object[] { cil[operandStart] };
                }

            case OperandType.ShortInlineR:
                advance += 4;
                if (op == OpCodes.Ldc_R4)
                {
                    return new object[] { ReadSingle(cil, operandStart) };
                }
                else
                {
                    return new object[] { ReadShort(cil, operandStart) };
                }

            case OperandType.ShortInlineVar:
                advance += 1;
                return new object[] { cil[operandStart] };

            default: throw new Exception("Unexpected operand type [" + op.OperandType + "]");
        }
    }
}
