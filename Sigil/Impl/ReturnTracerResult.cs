namespace ScrubJay.Sigil.Impl;

internal class ReturnTracerResult
{
    private class LabelEnumerableComparer : IEqualityComparer<IEnumerable<SigilLabel>>
    {
        public static readonly LabelEnumerableComparer Singleton = new LabelEnumerableComparer();

        private LabelEnumerableComparer() { }

        public bool Equals(IEnumerable<SigilLabel> x, IEnumerable<SigilLabel> y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            if (x.Count() != y.Count()) return false;

            using(var eX = x.GetEnumerator())
            using (var eY = y.GetEnumerator())
            {
                while (eX.MoveNext() && eY.MoveNext())
                {
                    if (eX.Current != eY.Current) return false;
                }
            }

            return true;
        }

        public int GetHashCode(IEnumerable<SigilLabel> obj)
        {
            if (obj == null) return 0;

            var ret = 0;

            foreach (var label in obj)
            {
                ret ^= label.GetHashCode();
            }

            return ret;
        }
    }

    public bool IsSuccess { get; private set; }
    public IEnumerable<IEnumerable<SigilLabel>> FailingPaths { get; private set; }

    private ReturnTracerResult() { }

    public static ReturnTracerResult Success()
    {
        return
            new ReturnTracerResult
            {
                IsSuccess = true,
            };
    }

    public static ReturnTracerResult Failure(List<SigilLabel> path)
    {
        return
            new ReturnTracerResult
            {
                IsSuccess = false,

                FailingPaths = new [] { path.ToList() },
            };
    }

    public static ReturnTracerResult Combo(params ReturnTracerResult[] other)
    {
        var asArr = other;

        if (asArr.All(r => r.IsSuccess)) return Success();

        var allPaths = asArr.Where(r => !r.IsSuccess).SelectMany(r => r.FailingPaths).ToList();

        var uniqePaths = allPaths.Distinct(LabelEnumerableComparer.Singleton).ToList();

        return
            new ReturnTracerResult
            {
                IsSuccess = false,

                FailingPaths = uniqePaths,
            };
    }
}
