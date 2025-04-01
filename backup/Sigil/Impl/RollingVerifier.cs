using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal class RollingVerifier
{
    private List<VerifiableTracker> _currentlyInScope;
    private List<LinqStack<List<TypeOnStack>>> _currentlyInScopeStacks;

    private Dictionary<SigilLabel, List<VerifiableTracker>> _restoreOnMark;
    private Dictionary<SigilLabel, List<LinqStack<List<TypeOnStack>>>> _restoreStacksOnMark;

    private Dictionary<SigilLabel, List<VerifiableTracker>> _verifyFromLabel;

    private Dictionary<SigilLabel, Tuple<bool, LinqStack<List<TypeOnStack>>>> _stacksAtLabels;
    private Dictionary<SigilLabel, List<Tuple<bool, LinqStack<List<TypeOnStack>>>>> _expectedStacksAtLabels;

    private bool _markCreatesNewVerifier;

    /// From the spec [see section - III.1.7.5 Backward branch constraints]:
    ///   In particular, if that single-pass analysis arrives at an instruction, call it location X, that
    ///   immediately follows an unconditional branch, and where X is not the target of an earlier branch
    ///   instruction, then the state of the evaluation stack at X, clearly, cannot be derived from existing
    ///   information. In this case, the CLI demands that the evaluation stack at X be empty.
    ///
    /// In practice, DynamicMethods don't need to follow this rule *but* that doesn't mean stricter
    /// verification won't be needed elsewhere.
    ///
    /// If this is set, then an "expectation of empty stack" transition is inserted before unconditional branches
    /// where needed.
    private bool _usesStrictBranchVerification;

    private HashSet<SigilLabel> _mustBeEmptyWhenBranchedTo;

    public RollingVerifier(SigilLabel beginAt, bool strictBranchVerification)
    {
        _usesStrictBranchVerification = strictBranchVerification;

        _restoreOnMark = new Dictionary<SigilLabel, List<VerifiableTracker>>();
        _restoreStacksOnMark = new Dictionary<SigilLabel, List<LinqStack<List<TypeOnStack>>>>();
        _verifyFromLabel = new Dictionary<SigilLabel, List<VerifiableTracker>>();

        _stacksAtLabels = new Dictionary<SigilLabel, Tuple<bool, LinqStack<List<TypeOnStack>>>>();
        _expectedStacksAtLabels = new Dictionary<SigilLabel, List<Tuple<bool, LinqStack<List<TypeOnStack>>>>>();

        _mustBeEmptyWhenBranchedTo = new HashSet<SigilLabel>();

        EmptyCurrentScope();

        Add(new VerifiableTracker(beginAt), new LinqStack<List<TypeOnStack>>());
    }

    private void EmptyCurrentScope()
    {
        _currentlyInScope = new List<VerifiableTracker>();
        _currentlyInScopeStacks = new List<LinqStack<List<TypeOnStack>>>();
    }

    private void Add(VerifiableTracker tracker, LinqStack<List<TypeOnStack>> stack)
    {
        _currentlyInScope.Add(tracker);
        _currentlyInScopeStacks.Add(stack);

        if (_currentlyInScope.Count != _currentlyInScopeStacks.Count)
        {
            throw new Exception();
        }
    }

    private void AddRange(List<VerifiableTracker> trackers, List<LinqStack<List<TypeOnStack>>> stacks)
    {
        if (trackers.Count != stacks.Count)
        {
            throw new Exception();
        }

        for (var i = 0; i < trackers.Count; i++)
        {
            Add(trackers[i], stacks[i]);
        }
    }

    private void RemoveAt(int ix)
    {
        _currentlyInScope.RemoveAt(ix);
        _currentlyInScopeStacks.RemoveAt(ix);

        if (_currentlyInScope.Count != _currentlyInScopeStacks.Count)
        {
            throw new Exception();
        }
    }

    public virtual VerificationResult Mark(SigilLabel sigilLabel)
    {
        // This is, effectively, "follows an unconditional branch & hasn't been seen before"
        if (_markCreatesNewVerifier && _usesStrictBranchVerification && !_expectedStacksAtLabels.ContainsKey(sigilLabel))
        {
            _mustBeEmptyWhenBranchedTo.Add(sigilLabel);
        }

        if (_currentlyInScope.Count > 0)
        {
            _stacksAtLabels[sigilLabel] = GetCurrentStack();

            var verify = CheckStackMatches(sigilLabel);
            if (verify != null)
            {
                return verify;
            }
        }

        if (_markCreatesNewVerifier)
        {
            var newVerifier = new VerifiableTracker(sigilLabel, baseless: true);
            Add(newVerifier, new LinqStack<List<TypeOnStack>>());
            _markCreatesNewVerifier = false;
        }

        List<VerifiableTracker> restore;
        if (_restoreOnMark.TryGetValue(sigilLabel, out restore))
        {
            // don't copy, we want the *exact* same verifiers restore here
            AddRange(restore, _restoreStacksOnMark[sigilLabel]);
            _restoreOnMark.Remove(sigilLabel);
            _restoreStacksOnMark.Remove(sigilLabel);
        }

        var based = _currentlyInScope.OneOrDefault(f => !f.IsBaseless);
        based = based ?? _currentlyInScope.One();

        var fromLabel = new VerifiableTracker(sigilLabel, based.IsBaseless, based);
        var fromStack = _currentlyInScopeStacks[_currentlyInScope.IndexOf(based)];
        Add(fromLabel, CopyStack(fromStack));

        if (!_verifyFromLabel.ContainsKey(sigilLabel))
        {
            _verifyFromLabel[sigilLabel] = new List<VerifiableTracker>();
        }

        _verifyFromLabel[sigilLabel].Add(fromLabel);

        RemoveUnnecessaryVerifiers();

        return VerificationResult.Successful();
    }

    // Looks at CurrentlyInScope and removes any verifiers that are not necessary going forward
    private void RemoveUnnecessaryVerifiers()
    {
        // if anything's rooted, we only need one of them (since the IL stream being currently valid means they must in the future)
        var rooted = _currentlyInScope.Where(c => !c.IsBaseless && c.CanBePruned).ToList();
        if (rooted.Count >= 2)
        {
            for (var i = 1; i < rooted.Count; i++)
            {
                var toRemove = rooted[i];
                var ix = _currentlyInScope.IndexOf(toRemove);

                RemoveAt(ix);
            }
        }

        // remove any verifiers that have duplicate terminal stack states; we know that another verifier will do just as well, no need to verify the whole instruction stream again
        for (var i = _currentlyInScope.Count - 1; i >= 0; i--)
        {
            var curStack = _currentlyInScopeStacks[i];

            var otherMatch =
                _currentlyInScopeStacks
                    .Select(
                        (cx, ix) =>
                        {
                            if (ix == i) return -1;

                            if (curStack.Count != cx.Count) return -1;

                            if (!_currentlyInScope[ix].CanBePruned) return -1;

                            for (var j = 0; j < curStack.Count; j++)
                            {
                                var curFrame = curStack.ElementAt(j);
                                var cxFrame = cx.ElementAt(j);

                                if (curFrame.Count != cxFrame.Count) return -1;

                                curFrame = curFrame.OrderBy(_ => _).ToList();
                                cxFrame = cxFrame.OrderBy(_ => _).ToList();

                                for (var k = 0; k < curFrame.Count; k++)
                                {
                                    var curT = curFrame[k];
                                    var cxT = cxFrame[k];

                                    if (curT != cxT) return -1;
                                }
                            }

                            return ix;
                        }
                    ).Where(x => x != -1).OrderByDescending(_ => _).ToList();

            foreach (var o in otherMatch)
            {
                RemoveAt(o);

                if (o < i)
                {
                    i--;
                }
            }
        }
    }

    public virtual VerificationResult ReThrow()
    {
        return Throw();
    }

    public virtual VerificationResult Throw()
    {
        EmptyCurrentScope();
        _markCreatesNewVerifier = true;

        return VerificationResult.Successful();
    }

    public virtual VerificationResult Return()
    {
        EmptyCurrentScope();
        _markCreatesNewVerifier = true;

        return VerificationResult.Successful();
    }

    public virtual VerificationResult UnconditionalBranch(SigilLabel to)
    {
        // If we've recorded elsewhere that the label we're branching to *must* receive
        // an empty stack, then inject a transition that expects that
        if (_mustBeEmptyWhenBranchedTo.Contains(to))
        {
            var trans = new List<StackTransition>();
            trans.Add(new StackTransition(sizeMustBe: 0));

            var stackIsEmpty = Transition(new InstructionAndTransitions(null, null, trans));

            if (stackIsEmpty != null) return stackIsEmpty;
        }

        var intoVerified = VerifyBranchInto(to);
        if (intoVerified != null)
        {
            return intoVerified;
        }

        UpdateRestores(to);

        if (!_restoreOnMark.ContainsKey(to))
        {
            _restoreOnMark[to] = new List<VerifiableTracker>();
            _restoreStacksOnMark[to] = new List<LinqStack<List<TypeOnStack>>>();
        }

        if (!_expectedStacksAtLabels.ContainsKey(to))
        {
            _expectedStacksAtLabels[to] = new List<Tuple<bool, LinqStack<List<TypeOnStack>>>>();
        }
        _expectedStacksAtLabels[to].Add(GetCurrentStack());

        var verify = CheckStackMatches(to);
        if (verify != null)
        {
            return verify;
        }

        _restoreOnMark[to].AddRange(_currentlyInScope);
        _restoreStacksOnMark[to].AddRange(_currentlyInScopeStacks);

        EmptyCurrentScope();
        _markCreatesNewVerifier = true;

        return VerificationResult.Successful();
    }

    public virtual VerificationResult ConditionalBranch(params SigilLabel[] toLabels)
    {
        foreach(var to in toLabels)
        {
            var intoVerified = VerifyBranchInto(to);
            if (intoVerified != null)
            {
                return intoVerified;
            }

            UpdateRestores(to);

            if (!_restoreOnMark.ContainsKey(to))
            {
                if (!_restoreOnMark.ContainsKey(to))
                {
                    _restoreOnMark[to] = new List<VerifiableTracker>();
                    _restoreStacksOnMark[to] = new List<LinqStack<List<TypeOnStack>>>();
                }

                _restoreOnMark[to].AddRange(_currentlyInScope.Select(t => t.Clone()));
                _restoreStacksOnMark[to].AddRange(_currentlyInScopeStacks.Select(s => CopyStack(s)));
            }

            if (!_expectedStacksAtLabels.ContainsKey(to))
            {
                _expectedStacksAtLabels[to] = new List<Tuple<bool, LinqStack<List<TypeOnStack>>>>();
            }
            _expectedStacksAtLabels[to].Add(GetCurrentStack());

            var verify = CheckStackMatches(to);
            if (verify != null)
            {
                return verify;
            }
        }

        return VerificationResult.Successful();
    }

    private Tuple<bool, LinqStack<List<TypeOnStack>>> GetCurrentStack()
    {
        Tuple<bool, LinqStack<List<TypeOnStack>>> ret = null;
        for(var i = 0; i < _currentlyInScope.Count; i++)
        {
            var c = _currentlyInScope[i];
            var stack = _currentlyInScopeStacks[i];

            var stackCopy = CopyStack(stack);

            var innerRet = Tuple.Create(c.IsBaseless, stackCopy);

            if (ret == null || (innerRet.Item1 && !ret.Item1) || innerRet.Item2.Count > ret.Item2.Count)
            {
                ret = innerRet;
            }
        }

        return ret;
    }

    private VerificationResult CheckStackMatches(SigilLabel atSigilLabel)
    {
        if (!_stacksAtLabels.ContainsKey(atSigilLabel) || !_expectedStacksAtLabels.ContainsKey(atSigilLabel)) return null;

        var actual = _stacksAtLabels[atSigilLabel];
        var expectations = _expectedStacksAtLabels[atSigilLabel];

        foreach (var exp in expectations)
        {
            var mismatch = CompareStacks(atSigilLabel, actual, exp);
            if (mismatch != null)
            {
                return mismatch;
            }
        }

        return null;
    }

    private VerificationResult CompareStacks(SigilLabel sigilLabel, Tuple<bool, LinqStack<List<TypeOnStack>>> actual, Tuple<bool, LinqStack<List<TypeOnStack>>> expected)
    {
        if (!actual.Item1 && !expected.Item1)
        {
            if (expected.Item2.Count != actual.Item2.Count)
            {
                // Both stacks are based, so the wrong size is a serious error as well
                return VerificationResult.FailureUnderflow(sigilLabel, expected.Item2.Count);
            }
        }

        for (var i = 0; i < expected.Item2.Count; i++)
        {
            if (i >= actual.Item2.Count && !actual.Item1)
            {
                // actual is based and expected wanted a value, this is an UNDERFLOW
                return VerificationResult.FailureUnderflow(sigilLabel, expected.Item2.Count);
            }

            var expectedTypes = expected.Item2.ElementAt(i);
            List<TypeOnStack> actualTypes;
            if (i < actual.Item2.Count)
            {
                actualTypes = actual.Item2.ElementAt(i);
            }
            else
            {
                // Underflowed, but our actual stack is baseless; so we assume we're good until proven otherwise
                break;
            }

            bool typesMatch = false;
            foreach (var a in actualTypes)
            {
                foreach (var e in expectedTypes)
                {
                    typesMatch |= e.IsAssignableFrom(a);
                }
            }

            if (!typesMatch)
            {
                // Just went through what's on the stack, and we the types are known to not be compatible
                return VerificationResult.FailureTypeMismatch(sigilLabel, actualTypes, expectedTypes);
            }
        }

        return null;
    }

    private void UpdateRestores(SigilLabel l)
    {
        // current verify is branching to this label, go and update all the "will be restored" bits

        var curRoot = _currentlyInScope.OneOrDefault(f => !f.IsBaseless);
        curRoot = curRoot ?? _currentlyInScope.OrderByDescending(c => c.Iteration).One();

        foreach (var kv in _restoreOnMark)
        {
            foreach (var v in kv.Value.ToList())
            {
                if (v.BeganAt == l)
                {
                    var replacement = curRoot.Concat(v);

                    kv.Value.Remove(v);
                    kv.Value.Add(replacement);
                }
            }
        }
    }

    private VerificationResult VerifyBranchInto(SigilLabel to)
    {
        List<VerifiableTracker> onInto;
        if (!_verifyFromLabel.TryGetValue(to, out onInto)) return null;

        _verifyFromLabel.Remove(to);

        foreach (var c in _currentlyInScope)
        {
            foreach (var into in onInto)
            {
                var completedCircuit = c.Concat(into);

                var verified = completedCircuit.CollapseAndVerify();
                if (!verified.Success)
                {
                    return verified;
                }

                if (completedCircuit.IsBaseless)
                {
                    if (!_verifyFromLabel.ContainsKey(completedCircuit.BeganAt))
                    {
                        _verifyFromLabel[completedCircuit.BeganAt] = new List<VerifiableTracker>();
                    }

                    _verifyFromLabel[completedCircuit.BeganAt].Add(completedCircuit);
                }
            }
        }

        return null;
    }

    private LinqStack<List<TypeOnStack>> CopyStack(LinqStack<List<TypeOnStack>> toCopy)
    {
        var ret = new LinqStack<List<TypeOnStack>>(toCopy.Count);

        for (var i = toCopy.Count - 1; i >= 0; i--)
        {
            ret.Push(toCopy.ElementAt(i));
        }

        return ret;
    }

    public virtual VerificationResult Transition(InstructionAndTransitions legalTransitions)
    {
        var stacks = new List<LinqStack<List<TypeOnStack>>>();

        VerificationResult last = null;
        foreach (var x in _currentlyInScope)
        {
            var inner = x.Transition(legalTransitions);

            if (!inner.Success) return inner;

            last = inner;
            stacks.Add(CopyStack(inner.Stack));
        }

        _currentlyInScopeStacks = stacks;

        return last;
    }

    public virtual LinqStack<TypeOnStack> InferStack(int ofDepth)
    {
        return _currentlyInScope.One().InferStack(ofDepth);
    }
}
