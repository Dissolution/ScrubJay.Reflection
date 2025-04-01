using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal class VerifiableTracker
{
    public int Iteration { get { return _transitions.Count; } }

    public SigilLabel BeganAt { get; private set; }

    // When the stack is "unbased" or "baseless", underflowing it results in wildcards
    //   eventually they'll be fixed up to actual types
    public bool IsBaseless { get; private set; }
    private List<InstructionAndTransitions> _transitions = new List<InstructionAndTransitions>();

    private Dictionary<SigilLabel, int> _markedLabelsAtTransitions = new Dictionary<SigilLabel, int>();
    private Dictionary<SigilLabel, int> _branchesAtTransitions = new Dictionary<SigilLabel, int>();

    private LinqStack<List<TypeOnStack>> _startingStack = new LinqStack<List<TypeOnStack>>();

    public bool CanBePruned { get; private set; }

    public VerifiableTracker(SigilLabel beganAt, bool baseless = false, VerifiableTracker createdFrom = null, bool canBePruned = true)
    {
        IsBaseless = baseless;
        BeganAt = beganAt;
        CanBePruned = canBePruned;

        _markedLabelsAtTransitions[beganAt] = 0;

        if (createdFrom != null)
        {
            _startingStack = GetStack(createdFrom);
        }
    }

    internal VerifiableTracker Concat(VerifiableTracker other)
    {
        var branchTo = _branchesAtTransitions.ContainsKey(other.BeganAt) ? _branchesAtTransitions[other.BeganAt] : _transitions.Count;
        var shouldTake = branchTo != _transitions.Count;

        var trans = new List<InstructionAndTransitions>(branchTo + other._transitions.Count);

        if (shouldTake)
        {
            for (var i = 0; i < branchTo; i++)
            {
                trans.Add(_transitions[i]);
            }
        }
        else
        {
            trans.AddRange(_transitions);
        }

        trans.AddRange(other._transitions);

        var canReuseCache = branchTo == _transitions.Count && IsBaseless && _cachedVerifyStack != null;

        var ret =
            new VerifiableTracker(BeganAt, IsBaseless)
            {
                _startingStack = new LinqStack<List<TypeOnStack>>(_startingStack.Reverse()),
                _transitions = trans,
                _cachedVerifyStack = canReuseCache ? new LinqStack<List<TypeOnStack>>(_cachedVerifyStack.Reverse()) : null,
                _cachedVerifyIndex = canReuseCache ? _cachedVerifyIndex : null,
            };

        return ret;
    }

    public VerificationResult Transition(InstructionAndTransitions legalTransitions)
    {
        _transitions.Add(legalTransitions);
        var ret = CollapseAndVerify();

        // revert!
        if (!ret.Success)
        {
            _transitions.RemoveAt(_transitions.Count - 1);
        }

        return ret;
    }

    public int? GetInstructionIndex(int ix)
    {
        if (ix < 0 || ix >= _transitions.Count) throw new Exception("ix must be between 0 and " + (_transitions.Count - 1) + "; found " + ix);

        return _transitions[ix].InstructionIndex;
    }

    private static LinqStack<List<TypeOnStack>> GetStack(VerifiableTracker tracker)
    {
        var retStack = new LinqStack<List<TypeOnStack>>(tracker._startingStack.Reverse());

        foreach (var t in tracker._transitions)
        {
            UpdateStack(retStack, t, tracker.IsBaseless);
        }

        return retStack;
    }

    private static void UpdateStack(LinqStack<List<TypeOnStack>> stack, InstructionAndTransitions wrapped, bool isBaseless)
    {
        var legal = wrapped.Transitions;
        var instr = wrapped.Instruction;

        var legalSize = 0;

        legal.ForEach(
            t =>
            {
                legalSize += t.PushedToStack.Length;

                if (t.Before != null) t.Before(stack, isBaseless);
            }
        );

        if (legal.Any(l => l.PoppedFromStack.Any(u => u == TypeOnStack.Get<PopAllType>())))
        {
            if (instr.HasValue)
            {
                for (var i = 0; i < stack.Count; i++)
                {
                    var ix = stack.Count - i - 1;
                    stack.ElementAt(i).ForEach(y => y.Mark(wrapped, ix));
                }
            }

            stack.Clear();
        }
        else
        {
            var toPop = legal.First().PoppedCount;

            for (var j = 0; j < toPop && stack.Count > 0; j++)
            {
                var popped = stack.Pop();

                if (instr.HasValue)
                {
                    var ix = toPop - j - 1;
                    popped.ForEach(y => y.Mark(wrapped, ix));
                }
            }
        }

        var toPush = new List<TypeOnStack>(legalSize);
        var pushed = new HashSet<TypeOnStack>();
        for(var i = 0; i < legal.Count; i++)
        {
            foreach (var p in legal[i].PushedToStack)
            {
                if (pushed.Contains(p)) continue;

                toPush.Add(p);
                pushed.Add(p);
            }
        }

        if (toPush.Count > 0)
        {
            stack.Push(toPush);
        }
    }

    private List<StackTransition> GetLegalTransitions(List<StackTransition> ops, LinqStack<List<TypeOnStack>> runningStack)
    {
        var ret = new List<StackTransition>(ops.Count);

        for (var i = 0; i < ops.Count; i++)
        {
            var w = ops[i];

            if (w.PoppedFromStack.All(u => u == TypeOnStack.Get<PopAllType>()))
            {
                ret.Add(w);
                continue;
            }

            var onStack = runningStack.Peek(IsBaseless, w.PoppedCount);

            if (onStack == null)
            {
                continue;
            }

            if (w.PushedToStack.Any(p => p == TypeOnStack.Get<SamePointerType>()))
            {
                if (w.PushedToStack.Length > 1)
                {
                    throw new Exception("SamePointerType can be only product of a transition which contains it");
                }

                var shouldBePointer = LinqAlternative.SelectMany(onStack, p => p.Where(x => x.IsPointer || x == TypeOnStack.Get<WildcardType>())).Distinct().ToList();

                if (shouldBePointer.Count == 0) continue;
                w = new StackTransition(w.PoppedFromStack, new [] { shouldBePointer.Single() });
            }

            if (w.PushedToStack.Any(p => p == TypeOnStack.Get<SameByRefType>()))
            {
                if (w.PushedToStack.Length > 1)
                {
                    throw new Exception("SameByRefType can be only product of a transition which contains it");
                }

                var shouldBeByRef = LinqAlternative.SelectMany(onStack, p => p.Where(x => x.IsReference || x == TypeOnStack.Get<WildcardType>())).Distinct().ToList();

                if (shouldBeByRef.Count == 0) continue;
                w = new StackTransition(w.PoppedFromStack, new[] { shouldBeByRef.Single() });
            }

            bool outerContinue = false;

            for (var j = 0; j < w.PoppedCount; j++)
            {
                var shouldBe = w.PoppedFromStack[j];
                var actuallyIs = onStack[j];

                if (!actuallyIs.Any(a => shouldBe.IsAssignableFrom(a)))
                {
                    outerContinue = true;
                    break;
                }
            }

            if (outerContinue) continue;

            ret.Add(w);
        }

        return ret;
    }

    private LinqStack<List<TypeOnStack>> _cachedVerifyStack;
    private int? _cachedVerifyIndex;
    public VerificationResult CollapseAndVerify()
    {
        var runningStack = _cachedVerifyStack ?? new LinqStack<List<TypeOnStack>>(_startingStack.Reverse());

        int i = _cachedVerifyIndex ?? 0;

        for (; i < _transitions.Count; i++)
        {
            var wrapped = _transitions[i];
            var ops = wrapped.Transitions;

            if(ops.Any(o => o.StackSizeMustBe.HasValue))
            {
                if (ops.Count > 1)
                {
                    throw new Exception("Shouldn't have multiple 'must be size' transitions at the same point");
                }

                var doIt = ops[0];

                if(doIt.StackSizeMustBe != runningStack.Count)
                {
                    return VerificationResult.FailureStackSize(this, i, doIt.StackSizeMustBe.Value);
                }
            }

            var legal = GetLegalTransitions(ops, runningStack);

            if (legal.Count == 0)
            {
                var wouldPop = ops.GroupBy(g => g.PoppedFromStack.Length).Single().Key;

                if (runningStack.Count < wouldPop)
                {
                    return VerificationResult.FailureUnderflow(this, i, wouldPop, runningStack);
                }

                IEnumerable<TypeOnStack> expected;
                var stackI = FindStackFailureIndex(runningStack, ops, out expected);

                return VerificationResult.FailureTypeMismatch(this, i, stackI, expected, runningStack);
            }

            if (legal.GroupBy(g => new { a = g.PoppedCount, b = g.PushedToStack.Length }).Count() > 1)
            {
                throw new Exception("Shouldn't be possible; legal transitions should have same push/pop #s");
            }

            // No reason to do all this work again
            _transitions[i] = new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, legal);

            bool popAll = legal.Any(l => l.PoppedFromStack.Contains(TypeOnStack.Get<PopAllType>()));
            if (popAll && legal.Count() != 1)
            {
                throw new Exception("PopAll cannot coexist with any other transitions");
            }

            if(!popAll)
            {
                var toPop = legal.First().PoppedCount;

                if (toPop > runningStack.Count && !IsBaseless)
                {
                    return VerificationResult.FailureUnderflow(this, i, toPop, runningStack);
                }
            }

            bool isDuplicate = legal.Any(l => l.IsDuplicate);
            if (isDuplicate && legal.Count() > 1)
            {
                throw new Exception("Duplicate must be only transition");
            }

            if (isDuplicate)
            {
                if (!IsBaseless && runningStack.Count == 0) return VerificationResult.FailureUnderflow(this, i, 1, runningStack);

                var toPush = runningStack.Count > 0 ? runningStack.Peek() : new List<TypeOnStack>(new[] { TypeOnStack.Get<WildcardType>() });

                UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, new List<StackTransition>(new[] { new StackTransition(new TypeOnStack[0], toPush) })), IsBaseless);
            }
            else
            {
                UpdateStack(runningStack, new InstructionAndTransitions(wrapped.Instruction, wrapped.InstructionIndex, legal), IsBaseless);
            }
        }

        _cachedVerifyIndex = i;
        _cachedVerifyStack = runningStack;

        return VerificationResult.Successful(this, runningStack);
    }

    private int FindStackFailureIndex(LinqStack<List<TypeOnStack>> types, IEnumerable<StackTransition> ops, out IEnumerable<TypeOnStack> expected)
    {
        var stillLegal = new List<StackTransition>(ops);

        for (var i = 0; i < types.Count; i++)
        {
            var actuallyIs = types.ElementAt(i);

            var legal = stillLegal.Where(l => actuallyIs.Any(a => l.PoppedFromStack[i].IsAssignableFrom(a))).ToList();

            if (legal.Count == 0)
            {
                expected = stillLegal.Select(l => l.PoppedFromStack[i]).Distinct().ToList();
                return i;
            }

            stillLegal = new List<StackTransition>(legal);
        }

        throw new Exception("Shouldn't be possible");
    }

    public VerifiableTracker Clone()
    {
        return
            new VerifiableTracker(BeganAt)
            {
                IsBaseless = IsBaseless,
                _markedLabelsAtTransitions = new Dictionary<SigilLabel,int>(_markedLabelsAtTransitions),
                _branchesAtTransitions = new Dictionary<SigilLabel,int>(_branchesAtTransitions),
                _transitions = new List<InstructionAndTransitions>(_transitions),
                _startingStack = new LinqStack<List<TypeOnStack>>(_startingStack.Reverse()),
            };
    }

    // Returns the current stack *if* it can be inferred down to single types *and* is either based or verifiable to the given depth
    public LinqStack<TypeOnStack> InferStack(int ofDepth)
    {
        var res = CollapseAndVerify();

        if(res.Stack.Count < ofDepth) return null;

        var ret = new LinqStack<TypeOnStack>();
        for (var i = ofDepth - 1; i >= 0; i--)
        {
            var couldBe = res.Stack.ElementAt(i);

            if (couldBe.Count() > 1) return null;

            ret.Push(couldBe.Single());
        }

        return ret;
    }

    public override string ToString()
    {
        var ret = new StringBuilder();

        if (_startingStack.Count > 0)
        {
            ret.AppendLine(
                "starts with: " +
                string.Join(", ",
                    _startingStack.Select(s => "[" + string.Join(", or", s.Select(x => x.ToString()).ToArray()) + "]").ToArray()
                )
            );
        }

        for(var i = 0; i < _transitions.Count; i++)
        {
            var label = _markedLabelsAtTransitions.Where(kv => kv.Value == i).Select(kv => kv.Key).SingleOrDefault();

            if (label != null)
            {
                ret.AppendLine(label.Name + ":");
            }

            var tran = _transitions[i];

            ret.AppendLine(tran.ToString());
        }

        return ret.ToString();
    }
}
