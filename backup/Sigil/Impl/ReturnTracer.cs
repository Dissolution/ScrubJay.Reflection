namespace ScrubJay.Sigil.Impl;

internal class ReturnTracer
{
    private readonly List<Tuple<OpCode, SigilLabel, int>> _branches;
    private readonly Dictionary<SigilLabel, int> _marks;
    private readonly List<int> _returns;
    private readonly List<int> _throws;

    public ReturnTracer(List<Tuple<OpCode, SigilLabel, int>> branches, Dictionary<SigilLabel, int> marks, List<int> returns, List<int> throws)
    {
        _branches = branches;
        _marks = marks;
        _returns = returns;
        _throws = throws;
    }

    private static bool IsUnconditionalBranch(OpCode op)
    {
        return
            op == OpCodes.Br ||
            op == OpCodes.Br_S ||
            op == OpCodes.Leave ||
            op == OpCodes.Leave_S;
    }

    private readonly Dictionary<int, ReturnTracerResult> _cache = new Dictionary<int, ReturnTracerResult>();

    private ReturnTracerResult TraceFrom(int startAt, List<SigilLabel> path, HashSet<SigilLabel> pathLookup)
    {
        ReturnTracerResult cached;
        if (_cache.TryGetValue(startAt, out cached))
        {
            return cached;
        }

        var nextBranches = _branches.Where(b => b.Item3 >= startAt).GroupBy(g => g.Item3).OrderBy(x => x.Key).FirstOrDefault();
        var orReturn = _returns.Where(ix => ix >= startAt && (nextBranches != null ? ix < nextBranches.Key : true)).Count();
        var orThrow = _throws.Where(ix => ix >= startAt && (nextBranches != null ? ix < nextBranches.Key : true)).Count();

        if (orReturn != 0)
        {
            _cache[startAt] = cached = ReturnTracerResult.Success();
            return cached;
        }

        if (orThrow != 0)
        {
            _cache[startAt] = cached = ReturnTracerResult.Success();
            return cached;
        }

        if (nextBranches == null)
        {
            _cache[startAt] = cached = ReturnTracerResult.Failure(path);
            return cached;
        }

        var ret = new List<ReturnTracerResult>();

        foreach (var nextBranch in nextBranches)
        {

            if (pathLookup.Contains(nextBranch.Item2))
            {
                _cache[startAt] = cached = ReturnTracerResult.Success();
                ret.Add(cached);
                continue;
            }

            var branchOp = nextBranch.Item1;

            var branchTo = _marks[nextBranch.Item2];

            var removeFromPathAt = path.Count;
            path.Add(nextBranch.Item2);
            pathLookup.Add(nextBranch.Item2);

            var fromFollowingBranch = TraceFrom(branchTo, path, pathLookup);

            path.RemoveAt(removeFromPathAt);
            pathLookup.Remove(nextBranch.Item2);

            if (IsUnconditionalBranch(branchOp))
            {
                _cache[startAt] = cached = fromFollowingBranch;
                //return cached;
                ret.Add(cached);
                continue;
            }

            var fromFallingThrough = TraceFrom(startAt + 1, path, pathLookup);

            _cache[startAt] = cached = ReturnTracerResult.Combo(fromFallingThrough, fromFollowingBranch);

            ret.Add(cached);
        }

        _cache[startAt] = cached = ReturnTracerResult.Combo(ret.ToArray());
        return cached;
    }

    public ReturnTracerResult Verify()
    {
        var firstLabel = _marks.OrderBy(o => o.Value).First().Key;
        var firstIx = _marks[firstLabel];

        var path = new List<SigilLabel>();
        path.Add(firstLabel);
        var pathLookup = new HashSet<SigilLabel>(path);

        return TraceFrom(firstIx, path, pathLookup);
    }
}
