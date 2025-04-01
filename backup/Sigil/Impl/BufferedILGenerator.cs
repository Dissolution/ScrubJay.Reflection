using System.Globalization;
using System.Text.RegularExpressions;
using ScrubJay.Sigil.Extensions;
using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal class BufferedILGenerator<TDelegateType>
{
    public BufferedILInstruction this[int ix]
    {
        get
        {
            return _traversableBuffer[ix];
        }
    }

    public int Index { get { return _buffer.Count; } }

    private readonly List<Action<ILGenerator, bool, StringBuilder>> _buffer = new List<Action<ILGenerator, bool, StringBuilder>>();
    private readonly List<BufferedILInstruction> _traversableBuffer = new List<BufferedILInstruction>();
    internal List<Operation<TDelegateType>?> _operations = [];
    private readonly List<Func<int>> _instructionSizes = new List<Func<int>>();

    public BufferedILGenerator()
    {
    }

    public string UnBuffer(ILGenerator il)
    {
        var log = new StringBuilder();

        // First thing will always be a Mark for tracing purposes; no reason to actually do it
        for(var i = 2; i < _buffer.Count; i++)
        {
            var x = _buffer[i];

            x(il, false, log);
        }

        return log.ToString();
    }

    private readonly Dictionary<int, int> _lengthCache = new Dictionary<int, int>();

    private int LengthTo(int end)
    {
        if (end == 0)
        {
            return 0;
        }

        int cached;
        if (_lengthCache.TryGetValue(end, out cached))
        {
            return cached;
        }

        int runningTotal = 0;

        for (var i = 0; i < end; i++)
        {
            var s = _instructionSizes[i];

            runningTotal += s();

            _lengthCache[i + 1] = runningTotal;
        }

        cached = _lengthCache[end];

        return cached;
    }

    internal string[] Instructions(List<SigilLocal> locals)
    {
        var ret = new List<string>();

        var invoke = typeof(TDelegateType).GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

        var dynMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes);
        var il = dynMethod.GetILGenerator();

        var instrs = new StringBuilder();

        for(var i = 0; i < _buffer.Count; i++)
        {
            var x = _buffer[i];

            x(il, true, instrs);
            var line = instrs.ToString().TrimEnd();

            if (line.StartsWith(OpCodes.Ldloc_0.ToString()) ||
                line.StartsWith(OpCodes.Stloc_0.ToString()))
            {
                line += " // " + GetInScopeAt(locals, i)[0];
            }

            if (line.StartsWith(OpCodes.Ldloc_1.ToString()) ||
                line.StartsWith(OpCodes.Stloc_1.ToString()))
            {
                line += " // " + GetInScopeAt(locals, i)[1];
            }

            if (line.StartsWith(OpCodes.Ldloc_2.ToString()) ||
                line.StartsWith(OpCodes.Stloc_2.ToString()))
            {
                line += " // " + GetInScopeAt(locals, i)[2];
            }

            if (line.StartsWith(OpCodes.Ldloc_3.ToString()) ||
                line.StartsWith(OpCodes.Stloc_3.ToString()))
            {
                line += " // " + GetInScopeAt(locals, i)[3];
            }

            if (line.StartsWith(OpCodes.Ldloc_S.ToString()) ||
                line.StartsWith(OpCodes.Stloc_S.ToString()))
            {
                line += " // " + ExtractLocal(line, locals, i);
            }

            ret.Add(line);
            instrs.Length = 0;
        }

        return ret.ToArray();
    }

    private static Dictionary<int, SigilLocal> GetInScopeAt(List<SigilLocal> allLocals, int ix)
    {
        return
            allLocals
                .Where(
                    l =>
                        l.DeclaredAtIndex <= ix &&
                        (l.ReleasedAtIndex == null || l.ReleasedAtIndex > ix)
                ).ToDictionary(d => (int)d.Index, d => d);
    }

#if DOTNET5_2
        private static Regex _ExtractLocal = new Regex(@"\s+(?<locId>\d+)");
#else
    private static readonly Regex _extractLocal = new Regex(@"\s+(?<locId>\d+)", RegexOptions.Compiled);
#endif

    private static SigilLocal ExtractLocal(string from, List<SigilLocal> locals, int ix)
    {
        var match = _extractLocal.Match(from);

        var locId = match.Groups["locId"].Value;

        var lid = int.Parse(locId);

        return GetInScopeAt(locals, ix)[lid];
    }

    public int ByteDistance(int start, int stop)
    {
        var toStart = LengthTo(start);
        var toStop = LengthTo(stop);

        return toStop - toStart;
    }

    public void Remove(int ix)
    {
        if (ix < 0 || ix >= _buffer.Count)
        {
            throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + _buffer.Count);
        }

        _lengthCache.Clear();

        _instructionSizes.RemoveAt(ix);

        _buffer.RemoveAt(ix);

        _traversableBuffer.RemoveAt(ix);

        _operations.RemoveAt(ix);
    }

    public void Insert(int ix, OpCode op)
    {
        if (ix < 0 || ix > _buffer.Count)
        {
            throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + _buffer.Count);
        }

        _lengthCache.Clear();

        _instructionSizes.Insert(ix, () => InstructionSize.Get(op));

        _buffer.Insert(
            ix,
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op);
                }

                if (op.IsPrefix())
                {
                    log.Append(op.ToString());
                }
                else
                {
                    log.AppendLine(op.ToString());
                }
            }
        );

        _traversableBuffer.Insert(
            ix,
            new BufferedILInstruction
            {
                IsInstruction = op,
            }
        );

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[0] });
    }

    public void Emit(OpCode op)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op);
                }

                if (op.IsPrefix())
                {
                    log.Append(op.ToString());
                }
                else
                {
                    log.AppendLine(op.ToString());
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[0] });
    }

    public void Emit(OpCode op, byte b)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, b);
                }

                if (op.IsPrefix())
                {
                    log.Append(op + "" + b + ".");
                }
                else
                {
                    log.AppendLine(op + " " + b);
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { b } });
    }

    public void Emit(OpCode op, short s)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, s);
                }

                if (op.IsPrefix())
                {
                    log.Append(op + "" + s + ".");
                }
                else
                {
                    log.AppendLine(op + " " + s);
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { s } });
    }

    public void Emit(OpCode op, int i)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, i);
                }

                if (op.IsPrefix())
                {
                    log.Append(op + "" + i + ".");
                }
                else
                {
                    log.AppendLine(op + " " + i);
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { i } });
    }

    public void Emit(OpCode op, uint ui)
    {
        int asInt;
        unchecked
        {
            asInt = (int)ui;
        }

        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, asInt);
                }

                log.AppendLine(op + " " + ui);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { ui } });
    }

    public void Emit(OpCode op, long l)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, l);
                }

                log.AppendLine(op + " " + l);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { l } });
    }

    public void Emit(OpCode op, ulong ul)
    {
        long asLong;
        unchecked
        {
            asLong = (long)ul;
        }

        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, asLong);
                }

                log.AppendLine(op + " " + ul);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { ul } });
    }

    public void Emit(OpCode op, float f)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, f);
                }

                log.AppendLine(op + " " + f.ToString(CultureInfo.InvariantCulture));
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { f } });
    }

    public void Emit(OpCode op, double d)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, d);
                }

                log.AppendLine(op + " " + d.ToString(CultureInfo.InvariantCulture));
            });

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { d } });
    }

    public void Emit(OpCode op, MethodInfo method, IEnumerable<Type> parameterTypes)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, method);
                }

                var mtdString = method is MethodBuilder ? method.Name : method.ToString();

                log.AppendLine(op + " " + mtdString);
            }
        );

        var parameters = new List<Type>(parameterTypes);
        if(!method.IsStatic)
        {
            var declaring = method.DeclaringType;

            if (declaring.IsValueType)
            {
                declaring = declaring.MakePointerType();
            }

            parameters.Insert(0, declaring);
        }

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = method.ReturnType, MethodParameterTypes = parameters });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { method } });
    }

    public void Emit(OpCode op, ConstructorInfo cons, IEnumerable<Type> parameterTypes)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, cons);
                }

                var mtdString = cons is ConstructorBuilder ? cons.Name : cons.ToString();

                log.AppendLine(op + " " + mtdString);
            }
        );

        var parameters = new List<Type>(parameterTypes);
        var declaring = cons.DeclaringType;

        if (declaring.IsValueType)
        {
            declaring = declaring.MakePointerType();
        }

        parameters.Insert(0, declaring);


        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = typeof(void), MethodParameterTypes = parameters });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { cons } });
    }

    public void Emit(OpCode op, ConstructorInfo cons)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, cons);
                }

                log.AppendLine(op + " " + cons);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { cons } });
    }

    public void Emit(OpCode op, Type type)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, type);
                }

                log.AppendLine(op + " " + type);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { type } });
    }

    public void Emit(OpCode op, FieldInfo field)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, field);
                }

                log.AppendLine(op + " " + field);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { field } });
    }

    public void Emit(OpCode op, string str)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.Emit(op, str);
                }

                log.AppendLine(op + " '" + str.Replace("'", @"\'") + "'");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { str } });
    }

    public void Emit(OpCode op, Sigil.SigilLabel sigilLabel, out UpdateOpCodeDelegate update)
    {
        var localOp = op;

        update =
            newOpcode =>
            {
                _lengthCache.Clear();

                localOp = newOpcode;
            };

        _instructionSizes.Add(() => InstructionSize.Get(localOp));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    var l = sigilLabel.LabelDel(il);
                    il.Emit(localOp, l);
                }

                log.AppendLine(localOp + " " + sigilLabel);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { sigilLabel } });
    }

    public void Emit(OpCode op, Sigil.SigilLabel[] labels, out UpdateOpCodeDelegate update)
    {
        var localOp = op;

        update =
            newOpcode =>
            {
                _lengthCache.Clear();

                localOp = newOpcode;
            };

        _instructionSizes.Add(() => InstructionSize.Get(localOp, labels));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    var ls = labels.Select(l => l.LabelDel(il)).ToArray();
                    il.Emit(localOp, ls);
                }

                log.AppendLine(localOp + " " + Join(", ", labels));
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = labels });
    }

    internal static string Join<T>(string delimiter, IEnumerable<T> parts) where T: class
    {
        using (var iter = parts.GetEnumerator())
        {
            if (!iter.MoveNext()) return "";
            var sb = new StringBuilder();
            var next = iter.Current;
            if (next != null) sb.Append(next);
            while (iter.MoveNext())
            {
                sb.Append(delimiter);
                next = iter.Current;
                if (next != null) sb.Append(next);
            }
            return sb.ToString();
        }
    }
    public void Emit(OpCode op, Sigil.SigilLocal sigilLocal)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    var l = sigilLocal.LocalDel(il);
                    il.Emit(op, l);
                }

                log.AppendLine(op + " " + sigilLocal);
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = new object[] { sigilLocal } });
    }

    public void Emit(OpCode op, CallingConventions callConventions, Type returnType, Type[] parameterTypes)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.EmitCalli(op, callConventions, returnType, parameterTypes, null);
                }

                log.AppendLine(op + " " + callConventions + " " + returnType + " " + Join(" ", parameterTypes));
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = returnType, MethodParameterTypes = parameterTypes  });

        var paras = new List<object> { callConventions, returnType };
        paras.AddRange(parameterTypes);

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = paras.ToArray() });
    }

    public void EmitCall(OpCode op, MethodInfo method, IEnumerable<Type> parameterTypes, Type[] arglist)
    {
        _instructionSizes.Add(() => InstructionSize.Get(op));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.EmitCall(op, method, arglist);
                }

                var mtdString = method is MethodBuilder ? method.Name : method.ToString();

                log.AppendLine(op + " " + mtdString + " __arglist(" + Join(", ", arglist) + ")");
            }
        );

        var parameters = new List<Type>(parameterTypes);
        if (!method.IsStatic)
        {
            var declaring = method.DeclaringType;

            if (declaring.IsValueType)
            {
                declaring = declaring.MakePointerType();
            }

            parameters.Insert(0, declaring);
        }

        parameters.AddRange(arglist);

        var paras = new List<object> { method };
        paras.AddRange(arglist);

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = method.ReturnType, MethodParameterTypes = parameters });

        _operations.Add(new Operation<TDelegateType> { OpCode = op, Parameters = paras.ToArray() });
    }

    public void EmitCalli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] arglist)
    {
        _instructionSizes.Add(() => InstructionSize.Get(OpCodes.Calli));

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, arglist);
                }

                log.AppendLine(OpCodes.Calli + " " + callingConvention + " " + returnType + " " + Join(" ", (IEnumerable<Type>)parameterTypes) + " __arglist(" + Join(", ", arglist) + ")");
            }
        );

        var ps = new List<Type>(parameterTypes);
        ps.AddRange(arglist);

        _traversableBuffer.Add(new BufferedILInstruction { IsInstruction = OpCodes.Calli, MethodReturnType = returnType, MethodParameterTypes = ps });

        var paras = new List<object>() { callingConvention, returnType };
        paras.AddRange(parameterTypes);
        paras.AddRange(arglist);

        _operations.Add(new Operation<TDelegateType> { OpCode = OpCodes.Calli, Parameters = paras.ToArray() });
    }

    public DefineLabelDelegate BeginExceptionBlock()
    {
        ILGenerator forIl = null;
        System.Reflection.Emit.Label? l = null;

        DefineLabelDelegate ret =
            il =>
            {
                if (forIl != null && forIl != il)
                {
                    l = null;
                }

                if (l != null) return l.Value;

                forIl = il;
                l = forIl.BeginExceptionBlock();

                return l.Value;
            };

        _instructionSizes.Add(() => InstructionSize.BeginExceptionBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    ret(il);
                }

                log.AppendLine("--BeginExceptionBlock--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { StartsExceptionBlock = true });

        _operations.Add(null);

        return ret;
    }

    public void BeginCatchBlock(Type exception)
    {
        _instructionSizes.Add(() => InstructionSize.BeginCatchBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.BeginCatchBlock(exception);
                }

                log.AppendLine("--BeginCatchBlock(" + exception + ")--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { StartsCatchBlock = true });

        _operations.Add(null);
    }

    public void EndExceptionBlock()
    {
        _instructionSizes.Add(() => InstructionSize.EndExceptionBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.EndExceptionBlock();
                }

                log.AppendLine("--EndExceptionBlock--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { EndsExceptionBlock = true });

        _operations.Add(null);
    }

    public void EndCatchBlock()
    {
        _instructionSizes.Add(() => InstructionSize.EndCatchBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                log.AppendLine("--EndCatchBlock--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { EndsCatchBlock = true });

        _operations.Add(null);
    }

    public void BeginFinallyBlock()
    {
        _instructionSizes.Add(() => InstructionSize.BeginFinallyBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    il.BeginFinallyBlock();
                }

                log.AppendLine("--BeginFinallyBlock--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { StartsFinallyBlock = true });

        _operations.Add(null);
    }

    public void EndFinallyBlock()
    {
        _instructionSizes.Add(() => InstructionSize.EndFinallyBlock());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                log.AppendLine("--EndFinallyBlock--");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { EndsFinallyBlock = true });

        _operations.Add(null);
    }

    public DefineLabelDelegate DefineLabel()
    {
        ILGenerator forIl = null;
        System.Reflection.Emit.Label? l = null;

        DefineLabelDelegate ret =
            il =>
            {
                if(forIl != null && forIl != il)
                {
                    l = null;
                }

                if (l != null) return l.Value;

                forIl = il;
                l = forIl.DefineLabel();

                return l.Value;
            };

        _instructionSizes.Add(() => InstructionSize.DefineLabel());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    ret(il);
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { DefinesLabel = true });

        _operations.Add(null);

        return ret;
    }

    public void MarkLabel(Sigil.SigilLabel sigilLabel)
    {
        _instructionSizes.Add(() => InstructionSize.MarkLabel());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly, log) =>
            {
                if (!logOnly)
                {
                    var l = sigilLabel.LabelDel(il);
                    il.MarkLabel(l);
                }

                log.AppendLine();
                log.AppendLine(sigilLabel + ":");
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { MarksSigilLabel = sigilLabel });

        _operations.Add(null);
    }

    public DeclareLocalDelegate DeclareLocal(Type type)
    {
        ILGenerator forIl = null;
        LocalBuilder l = null;

        DeclareLocalDelegate ret =
            il =>
            {
                if(forIl != null && il != forIl)
                {
                    l = null;
                }

                if (l != null) return l;

                forIl = il;
                l = forIl.DeclareLocal(type);

                return l;
            };

        _instructionSizes.Add(() => InstructionSize.DeclareLocal());

        _lengthCache.Clear();

        _buffer.Add(
            (il, logOnly,log) =>
            {
                if (!logOnly)
                {
                    ret(il);
                }
            }
        );

        _traversableBuffer.Add(new BufferedILInstruction { DeclaresLocal = true });

        _operations.Add(null);

        return ret;
    }
}
